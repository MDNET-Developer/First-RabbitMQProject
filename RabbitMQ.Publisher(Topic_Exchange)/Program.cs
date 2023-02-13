using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.Publisher_Topic_Exchange_
{
    internal class Program
    {
        public enum LogNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            //Ilk once RabbitMQ-e baglanmaq ucun ConnectionFactory class-ni newlemek lazimdir.
            var connectionFactory = new ConnectionFactory();

            connectionFactory.Uri = new Uri("amqps://ckwazfzf:l-f-a2qMhowYvCYK_8PmYtzSbcVEa06f@woodpecker.rmq.cloudamqp.com/ckwazfzf");

            //Bir dene baglanti acaq
            using var createConnection = connectionFactory.CreateConnection();

            //Bir dene kanal yaradaq bu baglanti uzerinden
            var channel = createConnection.CreateModel();

            //Bu versiyada publisher quyruq yaratmayacaq 
            //Burada "durable:true" ona gore verdik ki, proqram baglansa bele yaranacaq exchange silinmesin

            channel.ExchangeDeclare("logs-topic", durable: true, type: ExchangeType.Topic);
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                //Bura teqdimatdir, hansi ki umumen duzulur ki, bizim root-keylerimiz ve queue-namelerimiz hansilardir. Hemcinin uygun olaraq bunlar yaradilir. Bazada oturur.
                //Topic Exchange novunde bizim quyruqlar subcriber terefinden yaradilacaq deyene biz publisher terefde evvelden quyruq yaratmiriq. Evvelden quyruq yaratmadigimiz ucun hazir olaraq acarlarda teqdim etmirik. Belelikle ele mesaj gonderen zaman randoom olaraq ele acarlari teyin edib gonderirik. Subcriber hemin acarlara uygun olaraq oz mesajini alacaq.
                
            });
            //Bura mesaj gonderme prosesidir. Hansi ki randoom olaraq mesaj atir. Quyruq adi olsun, root adi olsun randoom gonderme edir.
            Console.Write("Mesajınızı daxil edin---> ");
            string message = Console.ReadLine();
            Console.Write("Nece eded mesaj gondereceksiz ?");
            int n = int.Parse(Console.ReadLine());
            for (int i = 1; i < n + 1; i++)
            {
                var LogName1 = (LogNames)new Random().Next(1, 5);
                var LogName2 = (LogNames)new Random().Next(1, 5);
                var LogName3 = (LogNames)new Random().Next(1, 5);
                var route_key = $"{LogName1}.{LogName2}.{LogName3}";
                string allMessage = message + "." + i.ToString() + $" | LogType: {route_key}";
                //Gonderidiyimiz mesajlar byte tipinde gonderilir. Buna gorede byte-lasdirma etdik
                var messageBody = Encoding.UTF8.GetBytes(allMessage);

                //Burada digerinden ferqli olaraq excahnge movcuddur lakin quyruq publisherdan yaradilmaigi ucun  "" veya string.Empty kimi qeyd edirik.
                //Exchange isletmeydimiz ucun default exchange den istifade olunur ki, burada quyruq adi daha sonra qeyd olunur
                channel.BasicPublish("logs-topic", route_key, null, messageBody);
                System.Threading.Thread.Sleep(100);
                Console.WriteLine($"{i})'{allMessage}'  - mesajı ugurla gonderildi.");
                Console.WriteLine("\n");
            }


            Console.ReadLine();
        }
    }
}
