using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using demomvc.Models;
using System.Data.SQLite;

namespace demomvc.Controllers
{
    public class StudentController : Controller
    {
        [System.Flags]
        public enum HttpVerbs { POST, GET };
        IList<Student> studentList = new List<Student>() { };

        public StudentController()
        {
            ReadData();
        }

        public ActionResult Index()
        {
            // ReadData();
            return View(studentList);
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            var std = studentList.Where(s => s.StudentId == Id).FirstOrDefault();
            return View(std);
        }

        [HttpPost]
        public ActionResult Edit(Student std)
        {
            EditData(std);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Add(int Id)
        {
            var std = new Student();
            return View(std);
        }

        [HttpPost]
        public ActionResult Add(Student std)
        {
            var stdId = studentList.Max(s => s.StudentId);
            AddData(std,stdId+1);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int Id)
        {
            DeleteData(Id);
            return RedirectToAction("Index");
        }

        void ReadData()
        {
            SQLiteDataReader sqlite_datareader;
            using (SQLiteConnection con = new SQLiteConnection("Data Source= student; Version = 3; New = True; Compress = True; "))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Student;", con))
                {

                    sqlite_datareader = cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        int id = sqlite_datareader.GetInt16(0);
                        int age = sqlite_datareader.GetInt16(2);
                        string name = sqlite_datareader.GetString(1);

                        studentList.Add(new Student { StudentId = id, StudentName = name, Age = age });
                    }

                }
                con.Close();
            }
        }
        void EditData(Student std)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source= student; Version = 3; New = True; Compress = True; "))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("UPDATE Student SET StudentName = @name, Age = @age WHERE Student.StudentId = @id;", con))
                {

                    cmd.Parameters.AddWithValue("@name", std.StudentName);
                    cmd.Parameters.AddWithValue("@age", std.Age);
                    cmd.Parameters.AddWithValue("@id", std.StudentId);

                    cmd.ExecuteNonQuery();

                }
                con.Close();
            }
        }
        void DeleteData(int std)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source= student; Version = 3; New = True; Compress = True; "))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Student WHERE StudentId = @id;", con))
                {

                    cmd.Parameters.AddWithValue("@id", std);

                    cmd.ExecuteNonQuery();

                }
                con.Close();
            }
        }
        void AddData(Student std, int stdId)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source= student; Version = 3; New = True; Compress = True; "))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Student(StudentName, Age, StudentId) VALUES( @name, @age, @id);", con))
                {

                    cmd.Parameters.AddWithValue("@name", std.StudentName);
                    cmd.Parameters.AddWithValue("@age", std.Age);
                    cmd.Parameters.AddWithValue("@id", stdId);

                    cmd.ExecuteNonQuery();

                }
                con.Close();
            }
        }
    }
}
