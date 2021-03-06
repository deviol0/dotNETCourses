using System;

namespace ConsoleProject
{
    public static class MainMenu
    {
        public static void ChooseTask()
        {
            var choose = Convert.ToInt32(IocStatic.OutputMessage("Choose task"));
            switch (choose)
            {
                case 1:
                {
                    TaxiCompany.Ui.Menu();
                    break;
                }
                case 2:
                {
                    TextModelConsole.Ui.Menu();
                    break;
                }
                case 3:
                {
                    var stationUi = new AutomationStation.UI(new IoConsole());
                    stationUi.Menu();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}