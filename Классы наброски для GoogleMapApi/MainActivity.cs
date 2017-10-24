using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace GidForYou
{
	[Activity(Label = "GidForYou", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
	public class MainActivity : AppCompatActivity
	{
		ListView lv2;
		// Класс для преобразования строки в список
		public class items
		{
			[JsonProperty("response")]
			public List<categories> item { get; set; }
		}
		//  Клас для заполнения списка
		public class categories
		{
			[JsonProperty("id")]
			public int id { get; set; }
			[JsonProperty("name")]
			public string name { get; set; }
			[JsonProperty("shortDesc")]
			public string shortDesc { get; set; }
			[JsonProperty("longDesc")]
			public string longDesc { get; set; }
			[JsonProperty("longitude")]
			public double? longitude { get; set; }
			[JsonProperty("latitude")]
			public double? latitude { get; set; }
			[JsonProperty("likes")]
			public int likes { get; set; }
		}
		//Адаптер использующий список для вывода нужных данных
		public class HomeScreenAdapter : BaseAdapter<categories>
		{
			List<categories> categories = new List<categories>();
			Activity context;
			public HomeScreenAdapter(Activity context, List<categories> categories)	
					: base()
			{
				this.context = context;
				this.categories = categories;
			}
			public override long GetItemId(int position)
			{
				return position;
			}
			public override categories this[int position]
			{
				get { return categories[position]; }
			}
			public override int Count
			{
				get { return categories.Count; }
			}
			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				var item = categories[position];
				View view = convertView;
				if (view == null)
					view = context.LayoutInflater.Inflate(Resource.Layout.MonumentView, null);
					view.FindViewById<TextView>(Resource.Id.Text2).Text = item.name;
					view.FindViewById<TextView>(Resource.Id.Text3).Text = item.shortDesc;
					return view;
			}
		}
		//Главная функция
		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			//задаем URL для запроса
			string url = "http://opendata.dev.zennex.ru/monument";
			//получаем Документ возвращаемый функцией GetCategories
			JsonValue json = await GetCategories(url);
			//Конвертируем документ в строку
			var r = json.ToString();
			//Делаем Десирилизацию документа и переносим в новый документ
			var ready = JsonConvert.DeserializeObject<List<categories>>(r);
			//Задаем форму вывода для ListView lv2
			lv2 = FindViewById<ListView>(Resource.Id.List1);
			//Заполняем ListView lv2
			lv2.Adapter = new HomeScreenAdapter(this, ready);
			//Действие по клику на List
			lv2.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				//Console.WriteLine("position = " + e.Position + " latitude = " + ready[e.Position].latitude + "longitude = "+ ready[e.Position].longitude);
				//Заносим в переменную данные из ListView
				string myLatitude = ready[e.Position].latitude.ToString();
				double myLat = Double.Parse(myLatitude);
				//Заносим в переменную lat полученую ширину из ListView
				double? lat = ready[e.Position].latitude;
				//Заносим в переменную lon полученую высоту из ListView
				double? lon = ready[e.Position].longitude;
				//Задаем название и переменную для перехода на другой  экран
				Intent GoToMonument = new Intent(this, typeof(TestActivity));
				//Используем переменную для передачи переменной lat с использованием имени Lattitude
				GoToMonument.PutExtra("latitude",Convert.ToDouble(lat));
				//Используем переменную для передачи переменной lon с использованием имени longitude
				GoToMonument.PutExtra("longitude", Convert.ToDouble(lon));
				//Производим принудительное 
				this.StartActivity(GoToMonument);
			};
			//Начало функции нафигации
			var bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);//Отображение наавигации
			bottomNavigation.NavigationItemSelected += (s, e) =>
			{
			switch (e.Item.ItemId)
			{
			case Resource.Id.action_map://Показать картку
			Toast.MakeText(this, "Action Map Clicked", ToastLength.Short).Show();
			Intent Map = new Intent(this, typeof(TestActivity));
			this.StartActivity(Map);
			break;
			case Resource.Id.action_monuments://Показать список
			Toast.MakeText(this, "Action Monuments Clicked", ToastLength.Short).Show();
			break;
			case Resource.Id.action_info://Показать "О нас"
			Toast.MakeText(this, "Action Clicked", ToastLength.Short).Show();
			Intent About = new Intent(this, typeof(About_Actvity));
			this.StartActivity(About);
			break;
			}
			}; 
			//Конец функции навигации

		}
		private async Task<JsonValue> GetCategories(string url)
		{
			//создаем URL для вэб запроса
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
			request.ContentType = "application/json";
			request.Method = "GET";
			//отправляем запрос и ожидаем ответа
			using (WebResponse response = await request.GetResponseAsync())
			{
				//получаем потом из данных вэб-страницы
				using (Stream stream = response.GetResponseStream())
				{
				//используем потом для построения документа
					JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
					Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
					//Возвращаем полученный документ
					return jsonDoc;
				}
			}
		}
	}
}