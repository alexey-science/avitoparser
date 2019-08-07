using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Net;

namespace AvitoInformer
{
    public partial class Form1 : Form
    {

        private int timePer;
        private bool closed = false;
        public Form1()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            backgroundWorker1.WorkerReportsProgress = true;
            if (args.Length > 1) {
                if(args[1] == "run")
                {
                    closed = true;
                   
                }
            }
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar) sender;
            textBox3.Text = tb.Value.ToString(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var cbtnToggler = checkBox1;
            cbtnToggler.Appearance = Appearance.Button;
            cbtnToggler.TextAlign = ContentAlignment.MiddleCenter;
            cbtnToggler.MinimumSize = new Size(75, 25);
            using(var connect = avitoDB.Con())
            {
                try
                {
                    connect.Open();
                    string sql = "SELECT  * FROM categories";
                    SQLiteDataReader rd = avitoDB.ExecQuery(sql, connect);
                    while (rd.Read())
                    {
                        comboBox1.Items.Add(rd["name"]);
                    }
                    sql = "SELECT * FROM mainInfo";
                    rd = avitoDB.ExecQuery(sql, connect);
                    if (!rd.Read())
                    {
                      //TODO: Сделать обнуление автоинкримента
                    }

                    rd.Close();

                }catch(Exception mes)
                {
                    MessageBox.Show(mes.Message);
                }
            }

            list_Update();
            if (closed)
            {
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker1.ReportProgress(1);
            }
            
        }

        private void list_Update()
        {
            using (var connect = avitoDB.Con())
            {
                try
                {
                    connect.Open();
                    string sql = "";
                    SQLiteDataReader rd;
                    sql = "SELECT * FROM mainInfo";
                    rd = avitoDB.ExecQuery(sql, connect);
                    listBox1.Items.Clear();
                    while (rd.Read())
                    {

                        listBox1.Items.Add(rd["id"] + " | " + rd["keywords"] + " | " + rd["categories"] + " | " + rd["region"] + " | " + rd["email"]);

                    }
                    rd.Close();

                }
                catch (Exception mes)
                {
                    MessageBox.Show(mes.Message);
                }
            }
        }

        private void parseAd()
        {
            //DateTime dt = DateTime.Today;
            using (var connect = avitoDB.Con())
            {

                try
                {
                    connect.Open();
                    string sql = "SELECT * FROM mainInfo;";
                    var rd = avitoDB.ExecQuery(sql, connect);
                    while (rd.Read())
                    {
                        try
                        {
                            List<Ad> ads = new List<Ad>();
                            bool validAd = true;
                            int page = 0;
                            DateTime dateNow = DateTime.Now;
                            string dateqNow = string.Format("{0}/{1}/{2} {3}:{4}:00", dateNow.Day, dateNow.Month, dateNow.Year, (dateNow.Minute - 31 < 0) ? dateNow.Hour - 1 : dateNow.Hour, (dateNow.Minute - 31) < 0 ? 60 - (31 - dateNow.Minute) : (dateNow.Minute - 31));
                            sql = "SELECT * FROM lastquery WHERE id_mi = " + rd["un_id"].ToString();
                            var readdate = avitoDB.ExecQuery(sql, connect);
                            var dateq = readdate.Read() ? readdate["dateq"].ToString() : "";
                            while (validAd)
                            {
                                page++;
                                var url = "";
                                try
                                {
                                    url = new WebClient().DownloadString(string.Format("https://www.avito.ru/{0}/{1}?q={2}&p={3}", rd["region"], rd["categories"], rd["keywords"], page));
                                }
                                catch (WebException wex)
                                {
                                    //MessageBox.Show(wex.Message);
                                    break;
                                }

                                HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                                html.LoadHtml(url);
                                //*[@id="catalog"]/div[5]/div[1]/div[4]/div[1]/div[2]/div[1]
                                var n = html.DocumentNode.SelectNodes("//*[@id=\"catalog\"]/div[5]/div[1]/div[4]/div[1]/div[2]/div/div[contains(@class,\"item_table\") and contains(@class,\"c-b-0\") and @data-type=1]");

                                //MessageBox.Show("Я тут");

                                foreach (var i in n)
                                {

                                    var img = i.ChildNodes[1].SelectNodes("a[1]/img[1]");
                                    string imglink = "";
                                    if (img != null)
                                    {
                                        var valid = img[0].GetAttributeValue("data-srcpath", "none");
                                        byte[] b = Encoding.Default.GetBytes((valid != "none") ? img[0].GetAttributeValue("data-srcpath", "none") : img[0].GetAttributeValue("src", "none"));
                                        imglink = Encoding.UTF8.GetString(b);
                                    }

                                    var description = i.ChildNodes[5];
                                    var link = description.SelectNodes("h3[1]/a[1]");
                                    //*[@id="i764657860"]/div[3]/
                                    var adate = description.SelectNodes("*//div[contains(@class, \"date\")]");
                                    string datead = "";
                                    if (adate != null)
                                    {
                                        var date = adate[0].InnerText.Trim(' ', '\n');
                                        byte[] bdate = Encoding.Default.GetBytes(date);
                                        datead = service.getDate(Encoding.UTF8.GetString(bdate));

                                    }

                                    string linkad = "";
                                    string namead = "";
                                    if (link != null)
                                    {
                                        var name = link[0].InnerText;
                                        var refer = link[0].GetAttributeValue("href", "none");
                                        byte[] bref = Encoding.Default.GetBytes(refer);
                                        byte[] bname = Encoding.Default.GetBytes(name);
                                        linkad = Encoding.UTF8.GetString(bref);
                                        namead = Encoding.UTF8.GetString(bname);
                                    }

                                    if (datead != "none")
                                    {
                                        DateTime utcdatead = Convert.ToDateTime(datead);
                                        DateTime utcdateq = dateq.Length > 0 ? Convert.ToDateTime(dateq) : utcdatead;
                                        //MessageBox.Show(utcdatead.ToString());
                                        //MessageBox.Show(utcdateq.ToString());
                                         
                                        if ((utcdatead - utcdateq).TotalMilliseconds > 0)
                                        {
                                            var ad = new Ad();
                                            ad.imgLink = imglink;
                                            ad.linkad = linkad;
                                            ad.namead = namead;
                                            ad.datead = datead;
                                            ads.Add(ad);
                                        }
                                        else
                                        {
                                            //MessageBox.Show(page.ToString() + "  " + "DateSmall");
                                            validAd = false;
                                            break;
                                        }

                                    }
                                    else
                                    {
                                        //MessageBox.Show(page.ToString() + "  " + "noneDate");

                                        validAd = false;
                                        break;
                                    }


                                } // end froeach

                            } // end while(valid)
                            readdate.Close();
                            string text = "";

                            int count = 0;
                            
                            if (ads.Count > 0)
                            {
                                foreach (var ad in ads)
                                {
                                    count++;
                                    text += string.Format("<img src='{0}' /> <br> <p>{1}</p> <br> <a href='http://avito.ru{2}' >{3}</a> <br> <br> <br>", ad.imgLink, ad.datead, ad.linkad, ad.namead);
                                    if ((count == 50) && (ads.Count >= 50))
                                    {
                                        count = 0;
                                        try
                                        {   if (text.Length > 0)
                                            {
                                                service.SendMail(rd["email"].ToString(), rd["keywords"].ToString(), text);
                                                //MessageBox.Show("Я тут");
                                            }
                                        }
                                        catch (Exception exp)
                                        {
                                            //MessageBox.Show(exp.Message);

                                            continue;
                                        }
                                        text = "";
                                    }
                                }


                                if ((ads.Count < 50) || (count < 50) && (ads.Count * count > 0) && (text.Length > 0))
                                {
                                    try
                                    {
                                        service.SendMail(rd["email"].ToString(), rd["keywords"].ToString(), text);
                                        //MessageBox.Show("Я тут");
                                    }
                                    catch (Exception exp)
                                    {
                                        //MessageBox.Show(exp.Message);

                                        continue;
                                    }

                                }
                                ads.Clear();
                            }

                            try
                            {
                                sql = string.Format("UPDATE lastquery SET dateq = \"{0}\" WHERE id_mi = {1}", dateqNow, rd["un_id"]);
                                avitoDB.ExecNonQuery(sql, connect);
                                avitoDB.closeDispoe();
                                
                            }
                            catch (SQLiteException sexp)
                            {
                                // MessageBox.Show(sexp.Message);

                                continue;
                            }

                        }catch(Exception exp)
                        {
                           // MessageBox.Show(exp.Message);

                            continue;
                        }
                       
                    } // end while (read)

                    rd.Close();
                }
                catch (Exception mesex)
                {
                    throw new Exception(mesex.Message);
                }
             
            }// end using
        }
        


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label8.Text = "On";
                timePer = trackBar1.Value;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker1.ReportProgress(1);
                //parseAd();
            }
            else
            {
                label8.Text = "Off";
                timer1.Stop();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                avitoDB.closeDispoe();
                string qtext = textBox1.Text.Replace(" ", "+");
                string region = radioButton1.Checked ? "chelyabinskaya_oblast" : "rossiya";
                string email = textBox2.Text;
                string ruscat = comboBox1.Text;
                DateTime dt = DateTime.Today;
                string adPer = comboBox2.Text;
                DateTime dtq;
                switch (adPer)
                {
                    case "Вчера":
                        dtq = new DateTime(dt.Year, dt.Month, dt.Day-1, 0, 0, 0);
                        break;
                    case "Месяц":
                        dtq = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
                        break;
                    default:
                        dtq = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                        break;

                }
               
            
                using(var connect = avitoDB.Con()) {
                    try
                    {
                        connect.Open(); 
                        string sql;
                        var rnd = new Random();
                        int id = rnd.Next(1000) * qtext.Length;
                        sql = string.Format("SELECT * FROM categories WHERE name = \"{0}\"", ruscat);
                        SQLiteDataReader cs = avitoDB.ExecQuery(sql, connect);
                        string categoria = cs.Read() ? cs["translate"].ToString() : "";
                        sql = string.Format("INSERT INTO mainInfo(keywords, categories, region, email, un_id) VALUES (\"{0}\", \"{1}\", \"{2}\", \"{3}\", {4})", qtext, categoria, region, email, id);
                        avitoDB.ExecNonQuery(sql, connect);
                        sql = string.Format("INSERT INTO lastquery(dateq, id_mi) VALUES (\"{0}\", {1})", dtq.ToString(), id);
                        avitoDB.ExecNonQuery(sql, connect);
                        cs.Close();
                        connect.Close();
                        connect.Dispose();
                        avitoDB.closeDispoe();

                    }
                    catch (Exception mes)
                    {
                        MessageBox.Show(mes.Message);
                    }
                }

                list_Update();

            }
            else
            {
                MessageBox.Show("Сначала нужно остановить Информер!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            list_Update();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var connect = avitoDB.Con())
            {
                try
                {
                    connect.Open();
                    string[] infstr = listBox1.SelectedItem.ToString().Split(' ');
                    string id = infstr[0];
                    string sql = "SELECT * FROM mainInfo WHERE id =" + id;
                    var rd = avitoDB.ExecQuery(sql, connect);
                    string id_mi = rd.Read() ? rd["un_id"].ToString() : "0";
                    sql = string.Format("DELETE FROM lastquery WHERE id_mi ={0}; DELETE FROM mainInfo WHERE id = {1}", id_mi, id);
                    avitoDB.ExecNonQuery(sql, connect);
                    rd.Close();
                    avitoDB.closeDispoe();
                }catch(Exception m)
                {
                    MessageBox.Show(m.Message);
                }
            }

            list_Update();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            parseAd();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                label9.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                label9.Text = "Error: " + e.Error.Message;
            }
            else
            {
                label9.Text = "В ожидании";
            }

            if (!closed)
            {
                checkBox1.Enabled = true;
                timer1.Interval = timePer * 60 * 1000;
                timer1.Start();
            }
            else
            {
                Application.Exit();
            }
         
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
           
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker1.ReportProgress(1);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            checkBox1.Enabled = false;
            label9.Text = "Работаю";
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
