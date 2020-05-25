using System;

namespace Versus.Models
{
    public class UserExercises
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid PushUpsId { get; set; }
        public Exercise PushUps { get; set; } 
        public Guid PullUpsId { get; set; }
        public Exercise PullUps { get; set; }
        public Guid AbsId { get; set; }
        public Exercise Abs { get; set; }
        public Guid SquatsId { get; set; }
        public Exercise Squats { get; set; }
    }
}