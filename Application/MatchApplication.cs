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

            matchResume.Id = matchControl.Id;
            matchResume.FirstPlayer = matchControl.FirstPlayer;

            _memoryCache.Set("MatchControl", matchControl);

            return matchResume;
        }

        public string MatchMoviment(MatchMoviment matchMoviment)
        {

            MatchResume matchResume = (MatchResume)_memoryCache.Get(matchMoviment.Id.ToString());
            if (matchResume == null)
            {
                return "Partida não encontrada";
            }

            matchMoviment.Round = (int)_memoryCache.Get("Round");

            MatchControl matchControl = (MatchControl)_memoryCache.Get("MatchControl");

            if (!ValidaTurno(matchControl, matchMoviment))
                return "Não é turno do jogador";

            _memoryCache.Remove("MatchControl");

            Jogada(matchMoviment, matchResume, matchControl);

            return "Jogada Feita com Sucesso";
        }

        private void Jogada(MatchMoviment matchMoviment, MatchResume matchResume, MatchControl matchControl)
        {

            if (matchMoviment.Player.Equals("X"))
            {
                matchControl.PlayerXPositions = (Dictionary<int, Dictionary<int, int>>)_memoryCache.Get("PlayerXPositions");

                if (matchMoviment.Round.Equals(1) || matchControl.PlayerXPositions == null)
                {
                    matchControl.PlayerXPositions = new Dictionary<int, Dictionary<int, int>>();
                }
                _memoryCache.Remove("PlayerXPositions");

                Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();

                keyValuePairs.Add(matchMoviment.Position.x, matchMoviment.Position.y);
                matchControl.PlayerXPositions.Add(matchMoviment.Round, keyValuePairs);

                _memoryCache.Set("PlayerXPositions", matchControl.PlayerXPositions);
            }
            else if (matchMoviment.Player.Equals("O"))
            {
                matchControl.PlayerOPositions = (Dictionary<int, Dictionary<int, int>>)_memoryCache.Get("PlayerXPositions");

                if (matchMoviment.Round.Equals(1) || matchControl.PlayerOPositions == null)
                {
                    matchControl.PlayerOPositions = new Dictionary<int, Dictionary<int, int>>();

                }
                _memoryCache.Remove("PlayerOPositions");


                Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();

                keyValuePairs.Add(matchMoviment.Position.x, matchMoviment.Position.y);
                matchControl.PlayerOPositions.Add(matchMoviment.Round, keyValuePairs);

                _memoryCache.Set("PlayerOPositions", matchControl.PlayerOPositions);

            }

            ValidaJogada(matchMoviment, matchControl);

            matchControl.LastPlayer = matchMoviment.Player;

            CachePrepare(matchMoviment, matchResume, matchControl);

        }

        private void CachePrepare(MatchMoviment matchMoviment, MatchResume matchResume, MatchControl matchControl)
        {
            _memoryCache.Remove(matchMoviment.Id.ToString());

            _memoryCache.Set(matchMoviment.Id.ToString(), matchResume);

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

        private bool ValidaJogada(MatchMoviment matchMoviment, MatchControl matchControl)
        {
            int x = matchMoviment.Position.x;
            int y = matchMoviment.Position.y;




        }
    }
}
