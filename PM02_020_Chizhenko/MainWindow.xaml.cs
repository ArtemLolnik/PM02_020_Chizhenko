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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PM02_020_Chizhenko
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Magazini.Text = "20,15,35,30";
            Skladi.Text = "40,22,38";
            Matrica.Text = "5,4,6,3;7,3,3,2;9,5,2,6";
        }

        //Кнопка выполнения задачи
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            int[] demand = Array.ConvertAll(Magazini.Text.Split(','), int.Parse);
            int[] supply = Array.ConvertAll(Skladi.Text.Split(','), int.Parse);
            int[][] costMatrix = Matrica.Text.Split(';')
                .Select(row => row.Split(',').Select(int.Parse).ToArray())
                .ToArray();

            var result = SolveTransportProblem(demand, supply, costMatrix);

            // Вывод результата
            txtResult.Content = result.Item1 + "\n" + "Общая стоимость: " + result.Item2;
        }

        //Функция очистки полей
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //Защита от случайного удаления полей
            if (MessageBox.Show("Хотите очистить поля?","Вопрос",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                //Очистка полей
                Skladi.Text = "";
                Magazini.Text = "";
                Matrica.Text = "";
                
            }
        }
        public Tuple<string, int> SolveTransportProblem(int[] demand, int[] supply, int[][] costMatrix)
        {
            int totalDemand = demand.Sum();
            int totalSupply = supply.Sum();

            if (totalDemand != totalSupply)
            {
                return Tuple.Create("Ошибка: сумма потребностей не равна сумме предложений!", 0);
            }

            int numRows = costMatrix.Length;
            int numCols = costMatrix[0].Length;

            int[][] result = new int[numRows][];
            for (int i = 0; i < numRows; i++)
            {
                result[i] = new int[numCols];
            }

            int[] supplyRemaining = new int[supply.Length];
            Array.Copy(supply, supplyRemaining, supply.Length);

            int[] demandRemaining = new int[demand.Length];
            Array.Copy(demand, demandRemaining, demand.Length);

            int totalCost = 0;

            while (supplyRemaining.Any(x => x > 0) && demandRemaining.Any(x => x > 0))
            {
                int minCost = int.MaxValue;
                int minCostRow = -1;
                int minCostCol = -1;

                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (supplyRemaining[i] > 0 && demandRemaining[j] > 0 && costMatrix[i][j] < minCost)
                        {
                            minCost = costMatrix[i][j];
                            minCostRow = i;
                            minCostCol = j;
                        }
                    }
                }

                int amountTransferred = Math.Min(supplyRemaining[minCostRow], demandRemaining[minCostCol]);
                result[minCostRow][minCostCol] = amountTransferred;
                supplyRemaining[minCostRow] -= amountTransferred;
                demandRemaining[minCostCol] -= amountTransferred;

                totalCost += amountTransferred * costMatrix[minCostRow][minCostCol];
            }

            string resultString = "Опорный план: \n";

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    resultString += result[i][j] + "\t";
                }
                resultString += "\n";
            }

            return Tuple.Create(resultString, totalCost);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(txtResult.Content)=="Решение")
            {
                MessageBox.Show("Нажмите на кнопку Решить, а затем выгружайте в файл");
            }
            else
            {
                //Сохранение файла
                //Инициализация переменной для сохраненния в файл
                string text = Convert.ToString(txtResult.Content);
                //Нахождение пути
                string LADP = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string NTC = System.IO.Path.Combine(LADP, "Necron Temple Co");
                // Создание папки, если она не существует
                if (!Directory.Exists(NTC))
                {
                    Directory.CreateDirectory(NTC);
                }
                //Создание пути где будет располагаться файл
                string filePath = System.IO.Path.Combine(NTC, $"Z.txt");
                //Запись в файл, а также его последующее сохранение
                try
                {
                    File.WriteAllText(filePath, text);
                    //Уведомление о том, что данные сохранены           
                    MessageBox.Show("Данные были сохранены: " + filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex}");
                }
            }
        }
    }
}
