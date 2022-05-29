using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DB2.View
{
    /// <summary>
    /// Логика взаимодействия для WindowAgreement.xaml
    /// </summary>
    public partial class WindowAgreement : Window
    {
        public List<Person> ListPerson;
        public List<StatusAgreement> Liststatusagreement;
        public List<TypeAgreement> Listtypeagreement;

        DbContextOptions<ApplicationContext> options;
        public WindowAgreement()
        {
            InitializeComponent();
            var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder.SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            options = optionsBuilder.UseSqlServer(connectionString).Options;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                ListPerson = db.Persons.ToList<Person>();
                Liststatusagreement = db.StatusAgreements.ToList<StatusAgreement>();
                Listtypeagreement = db.TypeAgreements.ToList<TypeAgreement>();

            }
        }
        private void btOK_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
