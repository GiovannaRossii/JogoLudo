using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JogoLudo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bem-vindo ao Ludo!");

            // Obter o número de jogadores
            Console.Write("Digite o número de jogadores (2-4): ");
            int numJogadores = int.Parse(Console.ReadLine());

            // Criar o tabuleiro do Ludo
            Tabuleiro ludoTabuleiro = new Tabuleiro();

            // Criar jogadores
            List<Jogador> jogadores = new List<Jogador>();//essa linha está criando uma lista vazia para armazenar objetos da classe 
            for (int i = 0; i < numJogadores; i++)
            {
                Console.Write($"Digite o nome do jogador {i + 1}: ");
                string nomeJogador = Console.ReadLine();
                Jogador jogador = new Jogador(nomeJogador);
                jogadores.Add(jogador);
            }

            // Loop principal do jogo
            bool fimDeJogo = false; // indicando que o jogo está em andamento.
           

            while (!fimDeJogo) // o loop será interrompido quando for true
            { 
                foreach (Jogador jogador in jogadores)
                {
                    Console.WriteLine($"\nÉ a vez de {jogador.Nome} jogar.");

                    // Variáveis para controlar o lançamento do dado
                    int valorDado = 0;
                    int lancamentos = 0;

                    // Loop para permitir até três lançamentos consecutivos de 6
                    while (lancamentos < 1)
                    {
                        // Lançar o dado
                        valorDado = LancarDado();
                        Console.WriteLine($"Você lançou o dado e obteve o valor {valorDado}.");

                        // Verificar se o valor do dado é 6
                        if (valorDado == 6)
                        {
                            lancamentos++;
                            if (lancamentos <= 3)
                            {
                                Console.WriteLine("Escolha um nova peça ou mova uma peça já inserido no tabuleiro!");
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Verificar se o jogador pode mover algum peão
                    bool podeMover = false;
                    foreach (Peca peca in jogador.Pecas)
                    {
                        if (peca.PodeMover(valorDado))
                        {
                            podeMover = true;
                            break;
                        }
                    }

                    if (!podeMover)
                    {
                        Console.WriteLine("Nenhum movimento possível. Passando a vez.");
                        continue;
                    }

                    // Escolher uma peça para mover
                    Peca pecaSelecionada = null; //usada para armazenar a peça selecionada pelo jogador.
                   
                    while (pecaSelecionada == null) //continuará executando até que uma peça válida seja selecionada.
                    {
                        Console.Write("Digite o número da peça que deseja mover: ");
                        int numeroPeca = int.Parse(Console.ReadLine());
                        pecaSelecionada = jogador.ObterPeca(numeroPeca);
                        if (pecaSelecionada == null || !pecaSelecionada.PodeMover(valorDado))
                        {
                            Console.WriteLine("Peça inválida. Por favor, escolha outra.");
                            pecaSelecionada = null;
                        }
                    }

                    // Mover a peça
                    pecaSelecionada.Mover(valorDado);

                    // Verificar se houve captura de peça adversária
                    Peca pecaCapturada = ludoTabuleiro.VerificarCaptura(pecaSelecionada);
                    if (pecaCapturada != null)
                    {
                        Console.WriteLine($"Você capturou a peça do jogador {pecaCapturada.Proprietario.Nome}!");
                        pecaCapturada.RetornarInicio();
                        lancamentos = 0; // Zerar os lançamentos se houver captura
                    }

                    bool lancarNovamente = false;
                    if (pecaSelecionada.PontoOrigem)
                    {
                        Console.WriteLine("Peão chegou ao ponto de origem. Você pode lançar o dado novamente.");
                        lancarNovamente = true;
                    }

                    // Verificar se o jogador venceu
                    if (jogador.Venceu())
                    {
                        Console.WriteLine($"\n{jogador.Nome} venceu o jogo!");
                        fimDeJogo = true;
                        break;
                    }
                   
                }
            }

            Console.WriteLine("\nFim de jogo. Obrigado por jogar!");
            Console.ReadLine();
        }

        // Função para lançar o dado
        static int LancarDado()
        {
            Random random = new Random();
            return random.Next(1, 7); //retorna um número aleatório entre 1 e 6
        }
    }

    // Classe que representa o tabuleiro do Ludo
    class Tabuleiro
    {
        private List<Peca> pecasEmJogo; // armazena as peças em jogo no tabuleiro.

        public Tabuleiro()
        {
            pecasEmJogo = new List<Peca>();
        }

        // Verificar se uma peça pode ser capturada
        public Peca VerificarCaptura(Peca pecaMovida)
        {
            foreach (Peca peca in pecasEmJogo) //Dentro do loop, é feita uma verificação para cada peça peca na lista, exceto a própria peça movida (peca != pecaMovida), se ela está na mesma posição que a pecaMovida 
            {
                if (peca != pecaMovida && peca.Posicao == pecaMovida.Posicao)
                {
                    return peca;
                }
            }
            return null;
        }

        // Adicionar uma peça ao tabuleiro
        public void AdicionarPeca(Peca peca)
        {
            pecasEmJogo.Add(peca);
        }
    }

    // Classe que representa um jogador
    class Jogador
    {
        public string Nome { get; private set; }
        public List<Peca> Pecas { get; private set; }

        public Jogador(string nome)
        {
            Nome = nome;
            Pecas = new List<Peca>();
            for (int i = 1; i <= 4; i++)
            {
                Pecas.Add(new Peca(this, i));
            }
        }

        // Verificar se o jogador venceu
        public bool Venceu()
        {
            foreach (Peca peca in Pecas)
            {
                if (!peca.ChegouPosicaoFinal())
                {
                    return false;
                }
            }
            return true;
        }

        // Obter uma peça pelo número
        public Peca ObterPeca(int numeroPeca)
        {
            foreach (Peca peca in Pecas)
            {
                if (peca.Id == numeroPeca)
                {
                    return peca;
                }
            }
            return null;
        }
    }

    // Classe que representa uma peça
    class Peca
    {
        public int Id { get; private set; }
        public Jogador Proprietario { get; private set; }
        public int Posicao { get; private set; }
        public bool PontoOrigem { get; set; }

        public Peca(Jogador proprietario, int id)
        {
            Proprietario = proprietario;
            Id = id;
            Posicao = 0;
        }

        // Verificar se a peça pode ser movida com o valor do dado
        public bool PodeMover(int valorDado)
        {
            // A peça só pode ser movida se estiver no início e o dado for 6
            if (Posicao == 0 && valorDado == 6)
            {
                return true;
            }

            // A nova posição não deve ultrapassar a última casa
            int novaPosicao = Posicao + valorDado;
            return Posicao > 0 && novaPosicao <= 52;
        }

        // Mover a peça com o valor do dado
        public void Mover(int valorDado)
        {
            if (Posicao == 0 && valorDado == 6)
            {
                Posicao = 1;
            }
            else if (Posicao > 0)
            {
                Posicao += valorDado;
            }
        }

        // Verificar se a peça chegou na posição final
        public bool ChegouPosicaoFinal()
        {
            return Posicao == 52;
        }

        // Retornar a peça para o início
        public void RetornarInicio()
        {
            Posicao = 0;
        }
    }
}






