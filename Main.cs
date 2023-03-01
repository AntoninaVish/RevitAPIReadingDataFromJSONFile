using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitAPIReadingDataFromJSONFile
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand

    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //указываем файл с которым будем работать, это файл формата "Json files (*.json) | *.json"
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Json files (*.json) | *.json"
            };

            string filePath = string.Empty;

            //проверяем, что пользователь указал путь, операция успешна и тогда копируем наш файл
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }

            //если путь равен ничему или пустой, то возвращаем Result.Cancelled
            if (string.IsNullOrEmpty(filePath))
                return Result.Cancelled;

            //собрали все помещения
            var rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .Cast<Room>()
                .ToList();

            string json = File.ReadAllText(filePath); //считываем данные из текстового файла в формате "Json"

            //преобразуем формат "Json" с помощью метода DeserializeObject, указываем, что этот объект должен быть списком List<RoomData>,
            //указываем прочтенный (Json) файл
            List<RoomData> roomDataList = JsonConvert.DeserializeObject<List<RoomData>>(json); //таким образом получаем список


            return Result.Succeeded;
        }
    }
}
