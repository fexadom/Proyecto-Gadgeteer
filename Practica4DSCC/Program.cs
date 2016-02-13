using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

//Estas referencias son necesarias para usar GLIDE
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using System.IO;
using System.Text;
using System.Xml;


namespace Practica4DSCC
{
    public partial class Program
    {
        //Objetos de interface gráfica GLIDE
        private GHI.Glide.Display.Window pantallaTiempo;
        private GHI.Glide.Display.Window pantallaTemperatura;
        private GHI.Glide.Display.Window pantallaNoticias;
        private GHI.Glide.Display.Window pantalla;
        private Button btn_temperature;
        private Button btn_news;
        String ip = "";
        // variables para pantalla tiempo
        private TextBlock texto;
        private TextBlock textotemp;
        private TextBlock fechatext;

        // variables para pantalla temperatura
        private TextBlock msm;
        private TextBlock temp;

        private Button atrastemp;

        // variables para news
        private Button atrasnews;
        private Button deportes;
        private Button cine;
        private Button economia;
        private TextBlock n1;
        private TextBlock n2;
        private TextBlock labeld;
        private TextBlock labelc;
        private TextBlock labele;
   
        private Button back;
        private Button next;

        String[] s2 = new String[15];
        String respuesta = "";
        Double tiempo = 0;
        String opcion = "";
        int intnoticias = 0;
        int ix = 0;
            private const string consignos = "áàäéèëíìïóòöúùuÁÀÄÉÈËÍÌÏÓÒÖÚÙÜçÇ‘’ñÑ";
        private const string sinsignos = "aaaeeeiiiooouuuAAAEEEIIIOOOUUUcC''nN";

        GT.Timer timer = new GT.Timer(1000); // every second (1000ms)

        GT.Timer timer2 = new GT.Timer(900); // every second (1000ms)
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
            ethernetJ11D.NetworkInterface.Open();
            ethernetJ11D.NetworkInterface.EnableDhcp();
            ethernetJ11D.UseThisNetworkInterface();
            //Carga la ventana principal
   
            pantallaTiempo = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.pantallaTiempo));
            pantallaTemperatura = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.pantallaTemperatura));
            pantallaNoticias = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.pantallaNoticias));
            GlideTouch.Initialize();

            //Inicializa el boton en la interface
            btn_temperature = (Button)pantallaTiempo.GetChildByName("clima");
            btn_news = (Button)pantallaTiempo.GetChildByName("noticias");
            deportes = (Button)pantallaNoticias.GetChildByName("deportes");
            cine = (Button)pantallaNoticias.GetChildByName("cine");
            economia = (Button)pantallaNoticias.GetChildByName("economia");
            atrastemp = (Button)pantallaTemperatura.GetChildByName("atras");
            atrasnews = (Button)pantallaNoticias.GetChildByName("atrasnews");
            //texto = (TextBlock)pantallaTiempo.GetChildByName("text_net_status");
            textotemp = (TextBlock)pantallaTiempo.GetChildByName("hora");
            fechatext = (TextBlock)pantallaTiempo.GetChildByName("fecha");
            msm = (TextBlock)pantallaTemperatura.GetChildByName("msm");
            temp = (TextBlock)pantallaTemperatura.GetChildByName("temp");
            n1 = (TextBlock)pantallaNoticias.GetChildByName("n1");
            n2 = (TextBlock)pantallaNoticias.GetChildByName("n2");
            labeld = (TextBlock)pantallaNoticias.GetChildByName("labeld");
            labelc = (TextBlock)pantallaNoticias.GetChildByName("labelc");
            labele = (TextBlock)pantallaNoticias.GetChildByName("labele");
            back = (Button)pantallaNoticias.GetChildByName("back");
            next = (Button)pantallaNoticias.GetChildByName("next");
            btn_temperature.TapEvent += btn_temperature_TapEvent;
            btn_news.TapEvent += btn_news_TapEvent;
           
            atrasnews.TapEvent += atrasnews_TapEvent;
            atrastemp.TapEvent += atrastemp_TapEvent;
            deportes.TapEvent += deportes_TapEvent;
            cine.TapEvent += cine_TapEvent;
            economia.TapEvent += economia_TapEvent;
            back.TapEvent += back_TapEvent;
            next.TapEvent += next_TapEvent;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            timer.Tick += timer_Tick;
            timer2.Tick += timer2_Tick;
           
            //Selecciona iniciarWindow como la ventana de inicio
            Glide.MainWindow = pantallaTiempo;
            timer2.Start();
        }


        void timer2_Tick(GT.Timer timer)
        {
            Glide.MainWindow = pantalla;
        }
        void tiempox() {
            fechatext.Text = "Esperando";
            textotemp.Text = "Esperando";
            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://api.timezonedb.com/?zone=America/Guayaquil&format=xml&key=RTHCFT8TVEL8");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();
            opcion = "tiempo";
        
        }
        void next_TapEvent(object sender)
        {
            if (intnoticias < 10 && intnoticias<ix+1)
            {
                intnoticias = intnoticias + 2;
                n1.Text = removerAcentos(s2[intnoticias]);
                n2.Text = removerAcentos(s2[intnoticias + 1]);
                Debug.Print(removerAcentos(s2[intnoticias]));
                Debug.Print(removerAcentos(s2[intnoticias + 1]));
            }
            else
            {
                intnoticias = 0;

            }
        }

        void back_TapEvent(object sender)
        {
            if (intnoticias > 1 && intnoticias <10)
            {
                intnoticias = intnoticias - 2;
                n1.Text = removerAcentos(s2[intnoticias]);
                n2.Text = removerAcentos(s2[intnoticias + 1]);
            }
        }

        void economia_TapEvent(object sender)
        {
            labele.ShowBackColor =true;
            labeld.ShowBackColor = false;
            labelc.ShowBackColor = false;
            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://www.eluniverso.com/rss/economia.xml");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();

        }

        void cine_TapEvent(object sender)
        {
            labelc.ShowBackColor = true;
            labeld.ShowBackColor = false;
            labele.ShowBackColor = false;
            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://www.eluniverso.com/rss/cine-y-tv.xml");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();
        }

        void deportes_TapEvent(object sender)
        {
            labeld.ShowBackColor = true;
            labele.ShowBackColor = false;
            labelc.ShowBackColor = false;
            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://www.eluniverso.com/rss/deportes.xml");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();
        }

        void atrasnews_TapEvent(object sender)
        {
            pantalla = pantallaTiempo;
            n1.Text = "";
            n2.Text = "";
        }

        void btn_news_TapEvent(object sender)
        {

            Debug.Print("news");
            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://www.eluniverso.com/rss/deportes.xml");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();
            opcion = "noticias";
            labeld.ShowBackColor = true;
            labele.ShowBackColor = false;
            labelc.ShowBackColor = false;
            pantalla = pantallaNoticias;
        }

        void atrastemp_TapEvent(object sender)
        {
            pantalla = pantallaTiempo;
            temp.Text = "";
            msm.Text = "";

        }

        void btn_temperature_TapEvent(object sender)
        {
            pantalla = pantallaTemperatura;
            Debug.Print("temperature");


            HttpRequest request = HttpHelper.CreateHttpGetRequest("http://api.openweathermap.org/data/2.5/weather?q=Guayaquil&mode=xml&appid=d3fb3b8eb0730860e148d39896bc1ee3&lang=es");
            request.ResponseReceived += request_ResponseReceived;
            request.SendRequest();
            opcion = "temperatura";

        }




        void timer_Tick(GT.Timer timer)
        {
            tiempo = tiempo + 1;
            respuesta = UnixTimeStampToDateTime(tiempo);
      
            // btn_inicio.Text = respuesta;

            String[] res = respuesta.Split(' ');
            String split = res[0];
            textotemp.Text = res[1];
            fechatext.Text = split;
         



        }

        public String UnixTimeStampToDateTime(double unixTimeStamp)
        {


            // First make a System.DateTime equivalent to the UNIX Epoch.
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the number of seconds in UNIX timestamp to be converted.
            dateTime = dateTime.AddSeconds(unixTimeStamp);

            // The dateTime now contains the right date/time so to format the string,
            // use the standard formatting methods of the DateTime object.
            String printDate = dateTime.ToString();
            return printDate;
        }

     
        public string removerAcentos(String texto)
        {
            if (!texto.Equals("") && (texto != null) && (!texto.Length.Equals(null)))
            {
                StringBuilder textoSinAcentos = new StringBuilder(texto.Length);
                int indexConAcento;
                foreach (char caracter in texto)
                {
                    indexConAcento = consignos.IndexOf(caracter);
                    if (indexConAcento > -1)
                        textoSinAcentos.Append(sinsignos.Substring(indexConAcento, 1));
                    else
                        textoSinAcentos.Append(caracter);
                }
                return textoSinAcentos.ToString();
            }
            else {
                ix = 0;
                return ""; }
        }
        void request_ResponseReceived(HttpRequest sender, HttpResponse response)
        {

            switch (opcion)
            {
                case "tiempo":
                    textotemp.Text = "Esperando ...";
                    timer.Start();
                    respuesta = response.Text;
                    Double horaLocal = 0;
                    String horaStr = "";
                    XmlReader xmL = XmlReader.Create(response.Stream);
                    while (xmL.Read())
                    {
                        if ((xmL.NodeType == XmlNodeType.Element) && (xmL.Name == "timestamp"))
                        {

                            horaStr = xmL.ReadElementString();
                            horaLocal = Double.Parse(horaStr);
                            horaStr = UnixTimeStampToDateTime(horaLocal);


                        }

                    }
                    respuesta = horaStr;
                    String[] res = respuesta.Split(' ');
                    tiempo = horaLocal;
                    fechatext.Text = res[0];
                    textotemp.Text = res[1];
                 
                    break;
                case "temperatura":

                    int celsius = 0;
                    Double C;
                    String celsiusStr = "";
                    String clima = "";
                    XmlReader xmLtemp = XmlReader.Create(response.Stream);
                    while (xmLtemp.Read())
                    {
                        if ((xmLtemp.NodeType == XmlNodeType.Element) && ((xmLtemp.Name == "temperature") || (xmLtemp.Name == "weather")))
                        {
                            if (xmLtemp.HasAttributes)
                            {
                                if (xmLtemp.Name == "temperature")
                                {
                                    celsiusStr = xmLtemp.GetAttribute("value");
                                    C = Double.Parse(celsiusStr);
                                    celsius = (int)(C - 273.15);
                                }
                                else if (xmLtemp.Name == "weather")
                                {
                                    clima = xmLtemp.GetAttribute("value");
                                }


                            }
                        }

                    }
                    temp.Text = celsius.ToString();
                    msm.Text = clima;
            
                    break;
                case "noticias":
             
                
                   
                    XmlReader rssXmlDoc = XmlReader.Create(response.Stream);
                    while (rssXmlDoc.Read())
                    {
                        if ((rssXmlDoc.NodeType == XmlNodeType.Element) && (rssXmlDoc.Name == "item"))
                        { 
                            intnoticias=0;
                            ix = 0;
                            while (rssXmlDoc.Read())
                            {
                               
                                if ((rssXmlDoc.NodeType == XmlNodeType.Element) && (rssXmlDoc.Name == "title"))
                                {
                                    if (ix < 10)
                                    {
                                        s2[ix] = rssXmlDoc.ReadElementString();

                                        ix++;
                                    }
                                }

                            }


                        }

                    }
                    Debug.Print(removerAcentos(s2[intnoticias]));
                    n1.Text = removerAcentos(s2[intnoticias]);
                    n2.Text = removerAcentos(s2[intnoticias+1]);
                 
                    break;
                default:
                    break;

            }

        }


        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
    
            ip = ethernetJ11D.NetworkSettings.IPAddress;
            //texto.Text = ip;
            tiempox();
            btn_news.Enabled = true;
            btn_temperature.Enabled = true;
            pantalla = pantallaTiempo;
       
        }

        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender, GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Desconectado");
            //texto.Text = "No Network";
            timer.Stop();
            btn_news.Enabled = false;
            btn_temperature.Enabled = false;

        }

    }
}
