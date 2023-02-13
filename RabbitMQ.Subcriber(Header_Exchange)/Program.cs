using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RabbitMQ.Subcriber_Header_Exchange_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Comsumer/Subcriber ==>");


            //Ilk once RabbitMQ-e baglanmaq ucun ConnectionFactory class-ni newlemek lazimdir.
            var connectionFactory = new ConnectionFactory();

            connectionFactory.Uri = new Uri("amqps://ckwazfzf:l-f-a2qMhowYvCYK_8PmYtzSbcVEa06f@woodpecker.rmq.cloudamqp.com/ckwazfzf");

            //Bir dene baglanti acaq
            using var createConnection = connectionFactory.CreateConnection();

            //Bir dene kanal yaradaq bu baglanti uzerinden
            var channel = createConnection.CreateModel();
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

            //Subcribere mesaj gelende bu event isleyecek(Recieved)
            //global true versek misal ucun 2 subcriber var 5 dene mesaj gonderirik 2 birine 3 birine verecek yeni ki tek seferde 5 mesaj getmelidir her ikisine, 6 desek 3 ona 3 buna bele bir sey. Yox eger false edek her biri ucun gonderecek
            channel.BasicQos(0, 1, false);

            //Musteri/abuneci yaradir
            var subcriber = new EventingBasicConsumer(channel);

            var queue_name = channel.QueueDeclare().QueueName;
            Dictionary<string, object> header = new();
            header.Add("format", "pdf");
            header.Add("shape", "a4");
            header.Add("x-match", "all");

            channel.QueueBind(queue_name, "header-exchange", string.Empty,header);


            //bu subcriber hansi quyruq ile isleyecek - 
            //autoAck- true yazsaq eger  biz bu mesaj gonderende subcribere catanda avtomatik quyruqdan silinir. False yazsaq eger silinme emeliyyati bizim elimizde olur
            channel.BasicConsume(queue_name, false, subcriber);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Working....");
            subcriber.Received += (object sender, BasicDeliverEventArgs e) =>
            {

                var receivedMessage = Encoding.UTF8.GetString(e.Body.ToArray());
                File.AppendAllText(@"C:\\Users\\Murad\\Desktop\\log-messageFILE.TXT", receivedMessage + "\n");
                Console.WriteLine($"Sizin yeni mesajınız var -->   {receivedMessage}");
                System.Threading.Thread.Sleep(555);
                //multiple = true/false ( true olsa eger islenmis amma gonderilmesmis mesajlar rabbitmq e melumat verer)
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
