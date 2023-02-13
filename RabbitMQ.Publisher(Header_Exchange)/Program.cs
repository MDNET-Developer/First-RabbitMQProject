using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Publisher_Header_Exchange_
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

            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);


            //Burada biz teqdm edeceyimiz format ve shape stringe aiddir, onlara qarsiliq olan pdf, a4 deyerleri object-e aiddir.
            Dictionary<string, object> header = new();
            header.Add("format", "pdf");
            header.Add("shape", "a4");

            var createdProperties = channel.CreateBasicProperties();
            createdProperties.Headers = header;

            Console.Write("Mesajınızı daxil edin---> ");
            string message = Console.ReadLine();


            //artiq burada root yaratmadigimiz ucun string.Empty veririk.
            channel.BasicPublish("header-exchange", string.Empty, createdProperties, Encoding.UTF8.GetBytes(message));
            Console.WriteLine("Mesaj ugurla gonderildi.....");

            //Bura mesaj gonderme prosesidir. Hansi ki randoom olaraq mesaj atir. Quyruq adi olsun, root adi olsun randoom gonderme edir.
           


        }
    }
}
