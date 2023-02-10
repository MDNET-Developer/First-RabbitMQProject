using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Publisher
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

            channel.QueueDeclare("donkey-queue", true, false, false);
            //("quyruq adi", bool durable = true/false(əgər false etsək butun quyruqlar yaddasda tutulur,əgər biz tətbiqə restart verəsək itəcək bu datalar. true versək əgər fiziki olaraq yadda saxlanilir və tətbiqə restart versək belə orada olacaq) ,bool exclusive =true/false (eger biz buna true yazsaq bu quyruq  yalniz bizim yaratdigimiz kanala qosulacaq,əgər biz istəsək subcriber tərəfində fərqli bir kanalada bağlansın o zaman false qeyd edəcəyik),bool autoDelete = true/false (əgər true qeyd etsək bizim son subcribe-mız çıxan zaman quyruq avtomatik silinəcək. False olsa orada saxlanılacaq)
            Console.Write("Mesajınızı daxil edin---> ");
            string message = Console.ReadLine();
            for (int i = 1; i < 56; i++)
            {
                string allMessage = message + i.ToString();
                //Gonderidiyimiz mesajlar byte tipinde gonderilir. Buna gorede byte-lasdirma etdik
                var messageBody = Encoding.UTF8.GetBytes(allMessage);
                //Ara vasiteci olaraq bizde exchange olmadigi ucun "" veya string.Empty kimi qeyd edirik.
                //Exchange isletmeydimiz ucun default exchange den istifade olunur ki, burada quyruq adi daha sonra qeyd olunur
                channel.BasicPublish(string.Empty, "donkey-queue", null, messageBody);
                System.Threading.Thread.Sleep(1555);
                Console.WriteLine($"{i})'{allMessage}'  - mesajı ugurla gonderildi :)");
                Console.WriteLine("-----------------------------------------------");
            }

            
            Console.ReadLine();

        }
    }
}
