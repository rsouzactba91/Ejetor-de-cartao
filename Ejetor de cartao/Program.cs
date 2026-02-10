using System;
using System.IO.Ports;
using System.Threading;

class Program
{
    static int Main(string[] args)
    {
        // Configurações da porta COM
        string portName = "COM1";
        int baudRate = 9600;
        int dataBits = 8;
        Parity parity = Parity.None;

        SerialPort? port = null;

        try
        {
            // MOSTRA as configurações (equivalente aos Labels do Forms)
            Console.WriteLine(
                $"Porta: {portName} | BaudRate: {baudRate} | DataBits: {dataBits} | Parity: {parity} | StopBits: {StopBits.One}"
            );

            // CONFIGURA a porta (SEM Console aqui)
            port = new SerialPort(portName, baudRate, parity, dataBits, StopBits.One)
            {
                ReadTimeout = 3000,
                WriteTimeout = 3000,
                DtrEnable = true,
                RtsEnable = true
            };

            // Abre a COM
            port.Open();
            Thread.Sleep(500);

            // Limpa buffers
            port.DiscardInBuffer();
            port.DiscardOutBuffer();

            // Comando real de ejeção (sniffado)
            byte[] cmd = { 0x02, 0x00, 0x02, 0x32, 0x30, 0x03, 0x01 };
            port.Write(cmd, 0, cmd.Length);

            Thread.Sleep(300);

            // ACK opcional (não bloqueia)
            if (port.BytesToRead > 0)
            {
                byte[] buffer = new byte[64];
                port.Read(buffer, 0, buffer.Length);
            }

            // ENQ
            port.Write(new byte[] { 0x05 }, 0, 1);
            Thread.Sleep(300);

            Console.WriteLine("Ejeção concluída com sucesso.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Erro: " + ex.Message);
            return 1;
        }
        finally
        {
            // Fecha e libera a COM SEMPRE
            if (port != null && port.IsOpen)
                port.Close();
        }
    }
}
