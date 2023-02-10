using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher_Fanout_Exchange_
{
    internal class Program
    {
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
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);


            Console.Write("Mesajınızı daxil edin---> ");
            string message = Console.ReadLine();
            Console.Write("Nece eded mesaj gondereceksiz ?");
            int n = int.Parse(Console.ReadLine());
            for (int i = 1; i < n+1; i++)
            {
                string allMessage = message + i.ToString();
                //Gonderidiyimiz mesajlar byte tipinde gonderilir. Buna gorede byte-lasdirma etdik
                var messageBody = Encoding.UTF8.GetBytes(allMessage);


                //Burada digerinden ferqli olaraq excahnge movcuddur lakin quyruq publisherdan yaradilmaigi ucun  "" veya string.Empty kimi qeyd edirik.
                //Exchange isletmeydimiz ucun default exchange den istifade olunur ki, burada quyruq adi daha sonra qeyd olunur
                channel.BasicPublish("logs-fanout", string.Empty, null, messageBody);
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"{i})'{allMessage}'  - mesajı ugurla gonderildi 🙂");
                Console.WriteLine("\n");
            }


            Console.ReadLine();
        }
    }
}
