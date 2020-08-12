using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace AutoCreator
{
    public partial class Form1 : Form
    {
        public OpenFileDialog opf = new OpenFileDialog();
        Editor editor = new Editor();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                opf.Filter = "JPEG файлы|*.jpg"; // Ограничивает выбор только jpg файлами
                opf.Title = "Выберите фотографию";
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    opf.OpenFile();
                    pictureBox1.ImageLocation = opf.FileName;
                }
                
           
            button2.Enabled = true; //кнопка "Изменить фото" становится доступна
            button3.Enabled = true; //кнопка "создать" становится доступна
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Editor editor = new Editor();
            editor.Owner = this; 
            editor.ShowDialog();
           
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image.Width == 768 && pictureBox1.Image.Height == 1024)
            {
                if (textBox2.TextLength != 0 && textBox3.TextLength != 0 && textBox4.TextLength != 0 || textBox1.TextLength != 0)
                {
                    var name = textBox3.Text;
                    var surname = textBox2.Text;
                    var patronymic = textBox4.Text;

                    Word.Application app = new Word.Application();
                  
                    opf.Filter = "Doc, Docx файлы|*.doc; *.docx"; // Ограничивает выбор только Doc файлами
                    opf.Title = "Выберите документ - шаблон";
                    if (opf.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                          //  System.IO.File.Copy(opf.FileName, Path.Combine(Path.GetDirectoryName(opf.FileName), "tmp" + ".docx"));
                            var WDocument = app.Documents.Open(opf.FileName);//Path.Combine(Path.GetDirectoryName(opf.FileName), "tmp" + ".docx"));
                            app.Visible = false;
                          //  WDocument.SaveAs2("Copy_Shab.docx", Word.WdSaveFormat.wdFormatDocumentDefault);
                            ReplaceWordStub("{name}", name, WDocument);
                            ReplaceWordStub("{surname}", surname, WDocument);
                            ReplaceWordStub("{patronymic}", patronymic, WDocument);
                            //!!!Здеся надо будет изменить поиск фотки которую нужно заменить, а то константным числом не комильфо (типа костыль)
                            for (var i = 1; i <= WDocument.Shapes.Count; i++)
                            {
                                var inlineShapeId = i;
                                if (inlineShapeId == 2)
                                {
                                    Word.Range ShAnc = WDocument.Shapes[2].Anchor; // Получаю якорь
                                    float He = WDocument.Shapes[2].Height, Wi = WDocument.Shapes[2].Width;
                                    float PosX = WDocument.Shapes[2].Left, PosY = WDocument.Shapes[2].Top;
                                    WDocument.Shapes.AddPicture(pictureBox1.ImageLocation, Left: PosX, Top: PosY, Width: Wi, Height: He, Anchor: ShAnc); //вставляю новое фото с пара-ми
                                    WDocument.Shapes[2].Delete();//стираю старую фотку
                                }
                            }
                            app.Visible = true;
                        }
                        catch
                        {
                            MessageBox.Show("Какая-то ошибка!");
                            app.Documents.Close();
                        }
                    }
                }
                else MessageBox.Show("Заполните поля ФИО либо поле табельный № !!", "Ошибка!", MessageBoxButtons.OK);
            }else MessageBox.Show("Фотография не соответствует критериям по разрешению и соотношению сторон. Пожалуйста отредактируйте фотографию!", "Ошибка!", MessageBoxButtons.OK);
        }   

        private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDoc)
        {//тут над изменить
            var range = wordDoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.TextLength != 0 && textBox3.TextLength != 0 && textBox4.TextLength != 0 || textBox1.TextLength != 0)
            {
                string serverName = "localhost"; // Адрес сервера (для локальной базы пишите "localhost")
                string userName = "root"; // Имя пользователя
                string dbName = "test"; //Имя базы данных
                string port = "3306"; // Порт для подключения
                string password = "root"; // Пароль для подключения
                string connStr = "server=" + serverName +
                    ";user=" + userName +
                    ";database=" + dbName +
                    ";port=" + port +
                    ";password=" + password + ";";
                string id = textBox1.Text;
                string sql = "SELECT * FROM test WHERE id = " + id; // Строка запроса
                MySqlConnection conn = new MySqlConnection(connStr);
                MySqlCommand sqlCom = new MySqlCommand(sql, conn);
                conn.Open();
                sqlCom.ExecuteNonQuery();// ТУТ НАДО БУДЕТ СЛОВИТЬ МОМЕНТ ОТСУТСТВИЯ ЗАПИСИ В ТАБЛИЦЕ
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);

                var myData = dt.Select();
                for (int i = 0; i < myData.Length; i++)
                {
                    for (int j = 0; j < myData[i].ItemArray.Length; j++)
                    {
                        textBox2.Text += myData[i].ItemArray[j+2];
                        textBox3.Text += myData[i].ItemArray[j+1];
                        textBox4.Text += myData[i].ItemArray[j+3];
                        pictureBox1.ImageLocation += myData[i].ItemArray[j + 4];
                        j = 5;
                    }
                }
                button2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                MessageBox.Show("Заполните поля ФИО либо поле табельный № !!", "Ошибка!", MessageBoxButtons.OK);
            }
        }
    }
}
