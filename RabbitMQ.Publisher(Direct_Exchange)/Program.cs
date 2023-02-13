using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;
using static RabbitMQ.Publisher_Direct_Exchange_.Program;

namespace RabbitMQ.Publisher_Direct_Exchange_
{
    internal class Program
    {
        public enum LogNames
        {
            Critical=1,
            Error=2,
            Warning=3,
            Info=4
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
            
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);
            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                //Bura teqdimatdir, hansi ki umumen duzulur ki, bizim root-keylerimiz ve queue-namelerimiz hansilardir. Hemcinin uygun olaraq bunlar yaradilir. Bazada oturur.
                var route_key = $"route-{x}";
                var queue_name = $"queue-{x}";
                channel.QueueDeclare(queue_name, true, false, false);
                channel.QueueBind(queue_name, "logs-direct", route_key, null);
            });
            //Bura mesaj gonderme prosesidir. Hansi ki randoom olaraq mesaj atir. Quyruq adi olsun, root adi olsun randoom gonderme edir.
            Console.Write("Mesajınızı daxil edin---> ");
            string message = Console.ReadLine();
            Console.Write("Nece eded mesaj gondereceksiz ?");
            int n = int.Parse(Console.ReadLine());
            for (int i = 1; i < n + 1; i++)
            {
                var LogName = (LogNames)new Random().Next(1, 5);
                string allMessage = message +"."+i.ToString() + $"| LogType: {LogName}";
                //Gonderidiyimiz mesajlar byte tipinde gonderilir. Buna gorede byte-lasdirma etdik
                var messageBody = Encoding.UTF8.GetBytes(allMessage);


                var route_key = $"route-{LogName}";

                //Burada digerinden ferqli olaraq excahnge movcuddur lakin quyruq publisherdan yaradilmaigi ucun  "" veya string.Empty kimi qeyd edirik.
                //Exchange isletmeydimiz ucun default exchange den istifade olunur ki, burada quyruq adi daha sonra qeyd olunur
                channel.BasicPublish("logs-direct", route_key, null, messageBody);
                System.Threading.Thread.Sleep(555);
                Console.WriteLine($"{i})'{allMessage}'  - mesajı ugurla gonderildi 🙂");
                Console.WriteLine("\n");
            }


            Console.ReadLine();
        }
    }
}
