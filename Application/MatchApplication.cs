using DTI_Challenge.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTI_Challenge.Application
{
    public class MatchApplication : IMatchApplication
    {
        public IMemoryCache _memoryCache { get; set; }
        public string msg { get; set; }

        public MatchApplication(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public MatchResume Start()
        {
            MatchResume matchResume = new MatchResume();
            MatchControl matchControl = new MatchControl
            {
                Id = Guid.NewGuid(),
            };


            matchControl.Random = new Random();
            int playerId = matchControl.Random.Next(0, 3);

            if (playerId.Equals(1) || playerId.Equals(3))
            {
                matchControl.FirstPlayer = matchControl.playerOne.PlayerLetter;
            }
            else
            {
                matchControl.FirstPlayer = matchControl.playerTwo.PlayerLetter;
            }

            MapeamentoIncial(matchControl);

            matchResume.Id = matchControl.Id;
            matchResume.FirstPlayer = matchControl.FirstPlayer;

            _memoryCache.Set("MatchControl", matchControl);

            return matchResume;
        }


        /// <summary>
        /// Mapeio todos os campos possiveis nesse game
        /// </summary>
        /// <param name="matchControl"></param>
        private void MapeamentoIncial(MatchControl matchControl)
        {
            matchControl.GamePositions = new Dictionary<int, MatchMap>();
            int i = 0;

            for (int a = 0; a <= 2; a++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    matchControl.GamePositions.TryAdd(i++, new MatchMap
                    {
                        X = a,
                        Y = j
                    });
                }

            }

        }

        /// <summary>
        /// Validações 
        /// </summary>
        /// <param name="matchMoviment"></param>
        /// <returns></returns>
        public string MatchMoviment(MatchMoviment matchMoviment)
        {
            this.msg = "Jogada Efetuada com Sucesso";

            MatchResume matchResume = (MatchResume)_memoryCache.Get(matchMoviment.Id.ToString());
            if (matchResume == null)
            {
                return "Partida não encontrada";
            }

            matchMoviment.Round = (int)_memoryCache.Get("Round");

            MatchControl matchControl = (MatchControl)_memoryCache.Get("MatchControl");

            if (!ValidaTurno(matchControl, matchMoviment))
                return "Não é turno do jogador";

            Jogada(matchMoviment, matchResume, matchControl);

            return msg;
        }

        private void Jogada(MatchMoviment matchMoviment, MatchResume matchResume, MatchControl matchControl)
        {


            if (matchMoviment.Player.Equals("X"))
            {
                MatchControl match = (MatchControl)_memoryCache.Get("MatchControl");

                MatchMap matchMap = new MatchMap()
                {
                    X = matchMoviment.Position.x,
                    Y = matchMoviment.Position.y
                };

                ValidarJogada(match, matchMap, matchMoviment);

            }
            else if (matchMoviment.Player.Equals("O"))
            {
                MatchControl match = (MatchControl)_memoryCache.Get("MatchControl");

                MatchMap matchMap = new MatchMap()
                {
                    X = matchMoviment.Position.x,
                    Y = matchMoviment.Position.y
                };
                ValidarJogada(match, matchMap, matchMoviment);
            }



            matchControl.LastPlayer = matchMoviment.Player;

            CachePrepare(matchMoviment, matchResume, matchControl);

        }

        private void ValidarJogada(MatchControl match, MatchMap matchMap, MatchMoviment matchMoviment)
        {
            foreach (var item in match.GamePositions)
            {
                if (item.Value.X.Equals(matchMap.X) && item.Value.Y.Equals(matchMap.Y))
                {
                    int key = item.Key;

                    if (item.Value.Player != null)
                    {
                        this.msg = "Campo já foi selecionado em outra rodada";
                        break;
                    }
                    match.GamePositions[key].Player = matchMoviment.Player;

                    break;
                }
            }
            
        }


        /// <summary>
        /// Metodo que carrega e exclui o Cache
        /// </summary>
        /// <param name="matchMoviment"></param>
        /// <param name="matchResume"></param>
        /// <param name="matchControl"></param>
        private void CachePrepare(MatchMoviment matchMoviment, MatchResume matchResume, MatchControl matchControl)
        {
            _memoryCache.Remove(matchMoviment.Id.ToString());

            _memoryCache.Set(matchMoviment.Id.ToString(), matchResume);

            _memoryCache.Remove("MatchControl");

            _memoryCache.Set("MatchControl", matchControl);

            _memoryCache.Remove("Round");

            matchMoviment.Round++;

            _memoryCache.Set("Round", matchMoviment.Round);
        }

        private bool ValidaTurno(MatchControl matchControl, MatchMoviment matchMoviment)
        {

            if ((matchMoviment.Round.Equals(1) && !matchControl.FirstPlayer.Equals(matchMoviment.Player))
                || matchMoviment.Player.Equals(matchControl.LastPlayer))
            {
                return false;
            }
            return true;
        }

        //private bool ValidaJogada(MatchMoviment matchMoviment, MatchControl matchControl)
        //{
        //    int x = matchMoviment.Position.x;
        //    int y = matchMoviment.Position.y;




        //}
    }
}
