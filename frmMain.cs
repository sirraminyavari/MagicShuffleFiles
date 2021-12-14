using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace MagicShuffleFiles
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            fbd.ShowDialog();

            string folder = fbd.SelectedPath.Trim();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;

            string[] file_names = Directory.GetFiles(folder);
            
            if (file_names.Length == 0)
            {
                MessageBox.Show("There is no file in the selected folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            MessageBox.Show(file_names.Length.ToString() + " Files selected. " + "Click OK to Start shuffling", "",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            
            ////////////////////////////////////////////////////////////////////////////
            //--------------------------------------------------------------------------
            foreach(string _fl in file_names)
            {
                int pos = _fl.LastIndexOf("\\");

                string _flFolder = pos < 0 ? string.Empty : _fl.Substring(0, pos + 1);
                string _flFile = pos < 0 ? _fl : _fl.Substring(pos + 1);

                new FileInfo(_fl).MoveTo(_flFolder + "0927456381 - " + _flFile);
            }
            //--------------------------------------------------------------------------
            ////////////////////////////////////////////////////////////////////////////

            List<FileName> fileNames = Directory.GetFiles(folder).Select(n => new FileName(n)).OrderBy(u => u.RND).ToList();

            int maxDigitsCount = num_lenght(fileNames.Count);

            for(int i = 0; i < fileNames.Count; ++i)
            {
                string zeros = "";
                for (int j = num_lenght(i + 1); j < maxDigitsCount; ++j)
                    zeros += "0";

                file_create(fileNames[i].Name, zeros + get_file_name(fileNames[i].Name, (i + 1).ToString()));
            }
            
            MessageBox.Show("The files created successfully", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    
        private static int num_lenght(int n)
        {
            return Convert.ToInt32(Math.Floor(Math.Log10(n))) + 1;
        }

        private static string modify_new_full_name(string newFullName)
        {
            if (!File.Exists(newFullName)) return newFullName;

            int pos = newFullName.LastIndexOf("\\");

            string folderName = pos < 0 ? string.Empty : newFullName.Substring(0, pos);
            string fileName = pos < 0 ? newFullName : newFullName.Substring(pos + 1);

            string fileNamePre = fileName;
            string fileNamePost = string.Empty;

            pos = fileName.IndexOf(' ');
            if (pos >= 0)
            {
                fileNamePre = fileName.Substring(0, pos);
                fileNamePost = fileName.Length > pos + 1 ? fileName.Substring(pos) : string.Empty;
            }
            else
            {
                pos = fileName.LastIndexOf('.');
                if (pos >= 0)
                {
                    fileNamePre = fileName.Substring(0, pos);
                    fileNamePost = fileName.Substring(pos);
                }
            }

            for (int i = 0; ; ++i)
            {
                newFullName = folderName + "\\" + fileNamePre + " - " + i.ToString() + fileNamePost;
                if (!File.Exists(newFullName)) break;
            }

            return newFullName;
        }

        private static void file_create(string fullname, string new_fullname)
        {
            FileInfo fi = new FileInfo(fullname);

            int pos = fullname.LastIndexOf("\\");
            string sourceFolder = pos < 0 ? string.Empty : fullname.Substring(0, pos);

            pos = new_fullname.LastIndexOf("\\");
            if (pos < 0)
            {
                new_fullname = sourceFolder + "\\" + new_fullname;
                pos = new_fullname.LastIndexOf("\\");
            }
            string destinationFolder = pos < 0 ? string.Empty : new_fullname.Substring(0, pos);

            try
            {
                if (!string.IsNullOrEmpty(sourceFolder) && sourceFolder == destinationFolder) 
                    fi.MoveTo(fullname == new_fullname ? new_fullname : modify_new_full_name(new_fullname));
                else 
                    fi.CopyTo(new_fullname);
            }
            catch { MessageBox.Show("Cannot access the files!", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private static string get_file_name(string file_name, string prefix)
        {
            int pos = file_name.LastIndexOf("\\");
            file_name = pos < 0 ? file_name : file_name.Substring(pos + 1);

            pos = file_name.LastIndexOf('.');
            string extension = pos >= 0 && file_name.Length > pos + 1 ? file_name.Substring(pos) : string.Empty;
            if (!string.IsNullOrEmpty(extension)) file_name = file_name.Substring(0, pos);

            file_name = (new Regex(@"^(\s*[0-9]+($|(\s*\.)|(\s+-)))+\s*")).Replace(file_name, string.Empty);

            return (file_name.Length == 0 ? prefix : (file_name[0] == '.' ? prefix + file_name : prefix + " - " + file_name)) + extension;
        }
        
        private static Random _RND = new Random((int)DateTime.Now.Ticks);

        private static int get_random_number(int min, int max)
        {
            return _RND.Next(min, max + 1);
        }

        private static int get_random_number(int length = 5)
        {
            return get_random_number((int)Math.Pow(10, (double)length - 1), (int)Math.Pow(10, (double)length) - 1);
        }

        private class FileName {
            public string Name;
            public int RND;

            public FileName(string name) {
                Name = name;
                RND = get_random_number();
            }
        }

        private void BtnStabilize_Click(object sender, EventArgs e)
        {
            fbd.ShowDialog();

            string folder = fbd.SelectedPath.Trim();
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder)) return;

            string[] file_names = Directory.GetFiles(folder);

            if (file_names.Length == 0)
            {
                MessageBox.Show("There is no file in the selected folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (string _fl in file_names.OrderBy(f => f))
            {
                int pos = _fl.LastIndexOf("\\");

                string _flFolder = pos < 0 ? string.Empty : _fl.Substring(0, pos + 1);
                string _flFile = pos < 0 ? _fl : _fl.Substring(pos + 1);

                string newFilePath = _flFolder + "r_" + _flFile;

                File.WriteAllBytes(newFilePath, File.ReadAllBytes(_fl));
                File.Delete(_fl);
                new FileInfo(newFilePath).MoveTo(_fl);

                Thread.Sleep(100);
            }

            MessageBox.Show("Done :)", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}