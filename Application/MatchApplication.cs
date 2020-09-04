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
            matchControl.GameVictoryPossibilities = new Dictionary<int, List<MatchMap>>();
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

            matchControl.GameVictoryPossibilities.Add(1, new List<MatchMap>
                {
                    new MatchMap { X = 0, Y = 2 },
                    new MatchMap { X = 0, Y = 1 },
                    new MatchMap { X = 0, Y = 0 }
                });

            matchControl.GameVictoryPossibilities.Add(2, new List<MatchMap>
                {
                   new MatchMap { X = 1, Y = 2 },
                   new MatchMap { X = 1, Y = 1 },
                   new MatchMap { X = 1, Y = 0 }
                });

            matchControl.GameVictoryPossibilities.Add(3, new List<MatchMap>
                {
                   new MatchMap { X = 2, Y = 2 },
                   new MatchMap { X = 2, Y = 1 },
                   new MatchMap { X = 2, Y = 0 }
                });

            matchControl.GameVictoryPossibilities.Add(4, new List<MatchMap>
                {
                   new MatchMap { X = 0, Y = 2 },
                   new MatchMap { X = 1, Y = 1 },
                   new MatchMap { X = 2, Y = 0 }
                });

            matchControl.GameVictoryPossibilities.Add(5, new List<MatchMap>
                {
                   new MatchMap { X = 2, Y = 2 },
                   new MatchMap { X = 1, Y = 1 },
                   new MatchMap { X = 0, Y = 0 }
                });


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
            if (ValidarJogada(matchControl, matchMoviment))
            {
                CachePrepare(matchMoviment, matchResume, matchControl);

                if (matchMoviment.Round >= 5)
                {
                    VerificaVitoria(matchControl);
                }
            }

        }

        private bool ValidarJogada(MatchControl matchControl, MatchMoviment matchMoviment)
        {
            MatchMap matchMap = new MatchMap()
            {
                X = matchMoviment.Position.x,
                Y = matchMoviment.Position.y
            };

            foreach (var item in matchControl.GamePositions)
            {
                if (item.Value.X.Equals(matchMap.X) && item.Value.Y.Equals(matchMap.Y))
                {
                    int key = item.Key;

                    if (item.Value.Player != null)
                    {
                        this.msg = "Campo já foi selecionado em outra rodada";
                        return false;
                    }
                    matchControl.GamePositions[key].Player = matchMoviment.Player;

                    break;

                }
            }
            return true;
        }


        /// <summary>
        /// Metodo que carrega e exclui o Cache
        /// </summary>
        /// <param name="matchMoviment"></param>
        /// <param name="matchResume"></param>
        /// <param name="matchControl"></param>
        private void CachePrepare(MatchMoviment matchMoviment, MatchResume matchResume, MatchControl matchControl)
        {
            matchControl.LastPlayer = matchMoviment.Player;

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


        /// <summary>
        /// Metodo Incompleto, Falta validar a vitoria
        /// </summary>
        /// <param name="matchControl"></param>
        private void VerificaVitoria(MatchControl matchControl)
        {
            List<MatchMap> matchPositionX = new List<MatchMap>();
            List<MatchMap> matchPositionO = new List<MatchMap>();

            Dictionary<int, List<MatchMap>> keyValuePairsX = new Dictionary<int, List<MatchMap>>();
            Dictionary<int, List<MatchMap>> keyValuePairO = new Dictionary<int, List<MatchMap>>();

            foreach (var item in matchControl.GamePositions)
            {
                if (item.Value.Player != null && item.Value.Player.Equals("X"))
                {
                    matchPositionX.Add(item.Value);
                }
                else if (item.Value.Player != null && item.Value.Player.Equals("O"))
                {
                    matchPositionO.Add(item.Value);

                }
            }

            ///Metodo de Validação incompleto
            ///falta finalziar a comparação dos resultados

            //if (matchPositionX.Count >= 3)
            //{
            //    keyValuePairsX.Add(1, matchPositionX);

            //    var winnerX = matchControl.GameVictoryPossibilities.DictionaryEquals(keyValuePairsX);
            //}
            //else if (matchPositionO.Count >= 3)
            //{

            //    keyValuePairO.Add(1, matchPositionO);
            //    var winnerO = matchControl.GameVictoryPossibilities.DictionaryEquals(keyValuePairO);

            //}

        }



    }
}
