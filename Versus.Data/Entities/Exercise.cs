using System;

namespace Versus.Data.Entities
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int HighScore { get; set; }
    }
}