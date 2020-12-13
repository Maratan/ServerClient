using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTCP
{
    class ClientObject
    {
        public TcpClient client;

        //конструктор класса
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();

                //получаем порции данных
                byte[] data = new byte[64]; //буфер для получаемых данных

                while (true)
                { 
                    //получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message =  builder.ToString();

                    Console.WriteLine(message);

                    //разбиваем строку на массив
                    string[] msgData = message.Split(':');

                    sFuncs allFuncs = new sFuncs();

                    switch (msgData[0])
                    { 
                        case "get":
                            if (allFuncs.sGet(msgData[1]))
                                message = "Get is TRUE!";
                            else
                                message = "Get is FALSE";
                            break;
                        case "post":
                            if (allFuncs.sPost(msgData[1]))
                                message = "Post is TRUE!";
                            else
                                message = "Post is FALSE";
                            break;
                        case "put":
                            if(!allFuncs.sPut(msgData[1]))
                                message = "Put is TRUE!";
                            else
                                message = "Put is FALSE";
                            break;
                        case "delete":
                            if(allFuncs.sDelete(msgData[1]))
                                message = "Delete is TRUE!";
                            else
                                message = "Delete is FALSE";
                            break;   
                    }

                    //отправляем обратно
                    message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();
                    data = Encoding.UTF8.GetBytes(message);

                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка:\n", ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();

                if (client != null)
                    client.Close();
            }
        }
    }
}
