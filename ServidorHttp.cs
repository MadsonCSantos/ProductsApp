using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Web;
class ServidorHttp
{
    private TcpListener Controlador {get; set;}
     private int Porta {get; set;}
     private int QtdeRequest {get; set;}
     public string HtmlExemplo { get; set; }
    

    public ServidorHttp(int porta = 8080)
    {
        this.Porta = porta;
        this.CriarHtmlExemplo();
        try
        {
            this.Controlador = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Porta);
            this.Controlador.Start();
            Console.WriteLine($"Servidor HTTP está rodando na porta {this.Porta}.");
            Console.WriteLine($"Para acessar, digite no navegador: http://localhost:{this.Porta}.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao iniciar servidor na porta {this.Porta}:\n{e.Message}.");
        }
    }
    private async Task AguardarRequests()
    {
        while(true)
        {
            Socket conexao = await this.Controlador.AcceptSocketAsync();
            this.QtdeRequest++;
        }
    }
    private void ProcessarRequest(Socket conexao, int numeroRequest)
    {
        Console.WriteLine($"Processar request #{numeroRequest}...\n");
        if(conexao.Connected)
        {
            byte[] bytesRequisicao = new byte[1024];
            conexao.Receive(bytesRequisicao, bytesRequisicao.Length, 0);
            string textoRequisicao = Encoding.UTF8.GetString(bytesRequisicao)
                .Replace((char)0, ' ').Trim();
                if(textoRequisicao.Length > 0)
                {
                    Console.WriteLine($"\n{textoRequisicao}\n");
                    var bytesConteudo = Encoding.UTF8.GetBytes(this.HtmlExemplo, 0, this.HtmlExemplo.Length);
                    var byCabecalho = GerarCabecalho("HTTP/1.1","text/html;charset=utf-8","200",0);
                    int bytesEnviados = conexao.Send(byCabecalho, byCabecalho.Length, byCabecalho.Length);
                    bytesEnviados += conexao.Send(bytesConteudo, bytesConteudo.Length, 0);
                    conexao.Close();
                    Console.WriteLine($"{bytesEnviados} bytes enviados em resposta à requisição #{numeroRequest}.");
                }
        }
        Console.WriteLine($"\nRequest {numeroRequest} finalizado.");
    }
    public byte[] GerarCabecalho(string versaoHttp, string tipoMine, string codigoHttp, int qtdByte = 0)
    {
        
    StringBuilder texto = new StringBuilder();
    texto.Append($"{versaoHttp} {codigoHttp}{Environment.NewLine}");
    texto.Append($"Server: Servidor Http Simples 1.0{Environment.NewLine}");
    texto.Append($"Content-Type: {tipoMine}{Environment.NewLine}");
    texto.Append($"Content-Length: {qtdByte}{Environment.NewLine}{Environment.NewLine}");
    return Encoding.UTF8.GetBytes(texto.ToString());
    }
    private void CriarHtmlExemplo()
    {
        StringBuilder html = new StringBuilder();
        html.Append("<!DOCTYPE html><html lang =\"pt-br\"><head><meta charset=\"UTF-8\">");
        html.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.Append("<title>Página Estática</title></head><body>");
        html.Append("<h1>Página Estática</h1></body></html>");
        this.HtmlExemplo = html.ToString();
    }
}