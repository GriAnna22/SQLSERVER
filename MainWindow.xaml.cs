using DB2.View;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DB2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DbContextOptions<ApplicationContext> options;

        // Коллекции клиентов, статусов и типов
        ObservableCollection<Agreement> agreements = new ObservableCollection<Agreement>();
        ObservableCollection<Person> persons = new ObservableCollection<Person>();
        ObservableCollection<StatusAgreement> statusAgreements = new ObservableCollection<StatusAgreement>();
        ObservableCollection<TypeAgreement> typeAgreements = new ObservableCollection<TypeAgreement>();


        public MainWindow()
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

        }


        /// <summary>
        /// Загрузка автомобилей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btLoad_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarAgreement.Visibility = Visibility.Visible;

            Task tc = Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    var query = db.Agreements
                    .Include(d => d.Person)
                    .Include(d => d.StatusAgreement)
                    .Include(d => d.TypeAgreement);

                    if (query.Count() != 0)
                    {
                        foreach (var c in query)
                            agreements.Add(c);
                    }
                }
            });

            await tc;
            await Task.Run(() => Thread.Sleep(200));
            ProgressBarAgreement.Visibility = Visibility.Collapsed;
            lvAgreement.ItemsSource = agreements;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            Agreement newAgreement = new Agreement();
            WindowAgreement editWindowAgreement = new WindowAgreement();
            editWindowAgreement.Title = "Добавление нового клиента";
            editWindowAgreement.DataContext = newAgreement;
            editWindowAgreement.ShowDialog();

            if (editWindowAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    try
                    {
                        db.Agreements.Add(newAgreement);
                        db.SaveChanges();
                        agreements.Add(newAgreement);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }

        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            Agreement edit = (Agreement)lvAgreement.SelectedItem;

            WindowAgreement editWindowAgreement = new WindowAgreement();
            editWindowAgreement.Title = "Редактирование данных по клиенту";
            editWindowAgreement.DataContext = edit;
            editWindowAgreement.ShowDialog();

            if (editWindowAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    Agreement agr = db.Agreements.Find(edit.Id);

                    if (agr.Number != edit.Number)
                        agr.Number = edit.Number;
                    if (agr.DataOpen != edit.DataOpen)
                        agr.DataOpen = edit.DataOpen;
                    if (agr.DataClouse != edit.DataClouse)
                        agr.DataClouse = edit.DataClouse;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            Agreement delAgreement = (Agreement)lvAgreement.SelectedItem;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                Agreement del = db.Agreements.Find(delAgreement.Id);

                if (del != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить данные по клиенту: \n" + del.Number + "  " + del.DataOpen + " " + del.DataClouse,
                  "Предупреждение", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        try
                        {
                            db.Agreements.Remove(del);
                            db.SaveChanges();
                            agreements.Remove(delAgreement);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                        }
                    }
                }
            }
        }

        private async void btLoadPer_Click(object sender, RoutedEventArgs e)
        {
            //using (ApplicationContext db = new ApplicationContext(options))
            //{
            //    var custs = db.Customers.ToList();
            //    lvCustomer.ItemsSource = custs;
            //}

            ProgressBarPerson.Visibility = Visibility.Visible;

            persons.Clear();

            Task tc = Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    var query = db.Persons;
                    if (query.Count() != 0)
                    {
                        foreach (var c in query)
                            persons.Add(c);
                    }
                }
            });

            await tc;

            await Task.Run(() => Thread.Sleep(200));

            ProgressBarPerson.Visibility = Visibility.Collapsed;
            lvPerson.ItemsSource = persons;
        }

        private void btAddPer_Click(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person();
            WindowPerson editWindowPerson = new WindowPerson();
            editWindowPerson.Title = "Добавление нового клиента";
            editWindowPerson.DataContext = newPerson;
            editWindowPerson.ShowDialog();

            if (editWindowPerson.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    try
                    {
                        db.Persons.Add(newPerson);
                        db.SaveChanges();
                        persons.Add(newPerson);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }

        }

        private void btEditPer_Click(object sender, RoutedEventArgs e)
        {
            Person editPer = (Person)lvPerson.SelectedItem;

            WindowPerson editWindowPerson = new WindowPerson();
            editWindowPerson.Title = "Редактирование данных по клиенту";
            editWindowPerson.DataContext = editPer;
            editWindowPerson.ShowDialog();



            if (editWindowPerson.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    Person per = db.Persons.Find(editPer.Id);

                    if (per.Inn != editPer.Inn)
                        per.Inn = editPer.Inn;
                    if (per.Type != editPer.Type)
                        per.Type = editPer.Type;
                    if (per.Shifer != editPer.Shifer)
                        per.Shifer = editPer.Shifer;
                    if (per.Data != editPer.Data)
                        per.Data = editPer.Data;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }

        }

        private void btDeletePer_Click(object sender, RoutedEventArgs e)
        {
            Person delPerson = (Person)lvPerson.SelectedItem;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                // Поиск в контексте удаляемого автомобиля
                Person delPer = db.Persons.Find(delPerson.Id);

                if (delPer != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить данные по клиенту: \n" + delPer.Inn + "  " + delPer.Type + " " + delPer.Shifer + " " + delPer.Data,
                  "Предупреждение", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        try
                        {
                            db.Persons.Remove(delPer);
                            db.SaveChanges();
                            persons.Remove(delPerson);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                        }
                    }
                }
            }
        }

        private async void btLoadSt_Click(object sender, RoutedEventArgs e)
        {
            //using (ApplicationContext db = new ApplicationContext(options))
            //{
            //    var ords = db.Orders
            //        .Include(c => c.Car)
            //        .Include(p => p.Cust)
            //        .ToList();

            //    lvOrder.ItemsSource = ords;
            //}

            ProgressBarStatusAgreement.Visibility = Visibility.Visible;


            Task tc = Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {

                    var query = db.StatusAgreements;
                    if (query.Count() != 0)
                    {
                        foreach (var c in query)
                            statusAgreements.Add(c);
                    }
                }
            });

            await tc;

            await Task.Run(() => Thread.Sleep(200));

            ProgressBarStatusAgreement.Visibility = Visibility.Collapsed;
            lvStatusAgreement.ItemsSource = statusAgreements;
        }

        private void btAddSt_Click(object sender, RoutedEventArgs e)
        {
            StatusAgreement newStatusAgreement = new StatusAgreement();
            WindowStatusAgreement editWindowStatusAgreement = new WindowStatusAgreement();
            editWindowStatusAgreement.Title = "Добавление нового клиента";
            editWindowStatusAgreement.DataContext = newStatusAgreement;
            editWindowStatusAgreement.ShowDialog();

            if (editWindowStatusAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    try
                    {
                        db.StatusAgreements.Add(newStatusAgreement);
                        db.SaveChanges();
                        statusAgreements.Add(newStatusAgreement);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }
        }

        private void btEditSt_Click(object sender, RoutedEventArgs e)
        {
            StatusAgreement editSt = (StatusAgreement)lvStatusAgreement.SelectedItem;

            WindowStatusAgreement editWindowStatusAgreement = new WindowStatusAgreement();
            editWindowStatusAgreement.Title = "Редактирование данных по клиенту";
            editWindowStatusAgreement.DataContext = editSt;
            editWindowStatusAgreement.ShowDialog();



            if (editWindowStatusAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    StatusAgreement st = db.StatusAgreements.Find(editSt.Id);

                    if (st.Status != editSt.Status)
                        st.Status = editSt.Status;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }
        }

        private void btDeleteSt_Click(object sender, RoutedEventArgs e)
        {
            StatusAgreement delStatusAgreement = (StatusAgreement)lvStatusAgreement.SelectedItem;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                // Поиск в контексте удаляемого автомобиля
                StatusAgreement delSt = db.StatusAgreements.Find(delStatusAgreement.Id);

                if (delSt != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить данные по клиенту: \n" + delSt.Status,
                  "Предупреждение", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        try
                        {
                            db.StatusAgreements.Remove(delSt);
                            db.SaveChanges();
                            statusAgreements.Remove(delStatusAgreement);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                        }
                    }
                }
            }
        }

        private async void btLoadTp_Click(object sender, RoutedEventArgs e)
        {
            //using (ApplicationContext db = new ApplicationContext(options))
            //{
            //    var ords = db.Orders
            //        .Include(c => c.Car)
            //        .Include(p => p.Cust)
            //        .ToList();

            //    lvOrder.ItemsSource = ords;
            //}

            ProgressBarTypeAgreement.Visibility = Visibility.Visible;

            Task tc = Task.Run(() =>
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {

                    var query = db.TypeAgreements;
                    if (query.Count() != 0)
                    {
                        foreach (var c in query)
                            typeAgreements.Add(c);
                    }
                }
            });

            await tc;

            await Task.Run(() => Thread.Sleep(200));

            ProgressBarTypeAgreement.Visibility = Visibility.Collapsed;
            lvTypeAgreement.ItemsSource = typeAgreements;
        }

        private void btAddTp_Click(object sender, RoutedEventArgs e)
        {
            TypeAgreement newTypeAgreement = new TypeAgreement();
            WindowTypeAgreement editWindowTypeAgreement = new WindowTypeAgreement();
            editWindowTypeAgreement.Title = "Добавление нового клиента";
            editWindowTypeAgreement.DataContext = newTypeAgreement;
            editWindowTypeAgreement.ShowDialog();

            if (editWindowTypeAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    try
                    {
                        db.TypeAgreements.Add(newTypeAgreement);
                        db.SaveChanges();
                        typeAgreements.Add(newTypeAgreement);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка добавления данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }
        }

        private void btEditTp_Click(object sender, RoutedEventArgs e)
        {
            TypeAgreement editTp = (TypeAgreement)lvTypeAgreement.SelectedItem;

            WindowTypeAgreement editWindowTypeAgreement = new WindowTypeAgreement();
            editWindowTypeAgreement.Title = "Редактирование данных по клиенту";
            editWindowTypeAgreement.DataContext = editTp;
            editWindowTypeAgreement.ShowDialog();



            if (editWindowTypeAgreement.DialogResult == true)
            {
                using (ApplicationContext db = new ApplicationContext(options))
                {
                    TypeAgreement tp = db.TypeAgreements.Find(editTp.Id);

                    if (tp.Type != editTp.Type)
                        tp.Type = editTp.Type;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\nОшибка редактирования данных!\n" + ex.Message, "Предупреждение");
                    }
                }
            }
        }

        private void btDeleteTp_Click(object sender, RoutedEventArgs e)
        {
            TypeAgreement delTypeAgreement = (TypeAgreement)lvTypeAgreement.SelectedItem;

            using (ApplicationContext db = new ApplicationContext(options))
            {
                // Поиск в контексте удаляемого автомобиля
                TypeAgreement delTp = db.TypeAgreements.Find(delTypeAgreement.Id);

                if (delTp != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить данные по клиенту: \n" + delTp.Type,
                  "Предупреждение", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        try
                        {
                            db.TypeAgreements.Remove(delTp);
                            db.SaveChanges();
                            typeAgreements.Remove(delTypeAgreement);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("\nОшибка удаления данных!\n" + ex.Message, "Предупреждение");
                        }
                    }
                }
            }
        }
    }
}
