using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMQ.Subcriber_Fanout_Exchange_
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Comsumer/Subcriber ==>");

            Console.ForegroundColor
          = ConsoleColor.Green;
            //Ilk once RabbitMQ-e baglanmaq ucun ConnectionFactory class-ni newlemek lazimdir.
            var connectionFactory = new ConnectionFactory();

            connectionFactory.Uri = new Uri("amqps://ckwazfzf:l-f-a2qMhowYvCYK_8PmYtzSbcVEa06f@woodpecker.rmq.cloudamqp.com/ckwazfzf");

            //Bir dene baglanti acaq
            using var createConnection = connectionFactory.CreateConnection();

            //Bir dene kanal yaradaq bu baglanti uzerinden
            var channel = createConnection.CreateModel();


            var randoomQueue = channel.QueueDeclare().QueueName;
            //channel.QueueDeclare("donkey-queue", true, false, false); Declare istifade etsek subcruberin yaratdigi quyruq hemise orada qalacaq. Yox eger Bind emeliyyatindan istifade etsek subcruber baglantini kesilen zaman quyruqda oradan silinecek.
            channel.QueueBind(randoomQueue, "logs-fanout", string.Empty, null);


            //Subcribere mesaj gelende bu event isleyecek(Recieved)
            //global true versek misal ucun 2 subcriber var 5 dene mesaj gonderirik 2 birine 3 birine verecek yeni ki tek seferde 5 mesaj getmelidir her ikisine, 6 desek 3 ona 3 buna bele bir sey. Yox eger false edek her biri ucun gonderecek
            channel.BasicQos(0, 1, false);

            //Musteri/abuneci yaradir
            var subcriber = new EventingBasicConsumer(channel);

           

            //bu subcriber hansi quyruq ile isleyecek - 
            //autoAck- true yazsaq eger  biz bu mesaj gonderende subcribere catanda avtomatik quyruqdan silinir. False yazsaq eger silinme emeliyyati bizim elimizde olur
            channel.BasicConsume(randoomQueue, false, subcriber);


            Console.WriteLine("Working....");
            subcriber.Received += (object sender, BasicDeliverEventArgs e) =>
            {

                var receivedMessage = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Sizin yeni mesajınız var 🙃 - {receivedMessage}");
                System.Threading.Thread.Sleep(1555);
                //multiple = true/false ( true olsa eger islenmis amma gonderilmesmis mesajlar rabbitmq e melumat verer)
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
