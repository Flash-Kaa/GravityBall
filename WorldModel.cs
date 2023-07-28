using System;
using System.Windows.Forms;

namespace GravityBalls
{
    public class WorldModel
    {
        const double Gravity = 9.8;
        const double ResisCoefForDistance = 10;
        public double BallRadius;
        public double BallX, BallY;
        public double WorldWidth, WorldHeight;

        public static int CountGreenGoal = 0;
        public static int CountRedGoal = 0;
        public static int countToWinGoals = 5;
        public static int indent = 1;
        public static double partOccupByGate = 0.1;

        static bool BallInField = false;

        double FallSpeed = 100, HorizSpeed = 100;
        double airResist = 0.1;

        public void SimulateTimeframe(double dt)
        {
            //меняем направление, если шар задел стену
            FallSpeed = Reverse(FallSpeed, BallY, BallRadius, WorldHeight);
            HorizSpeed = Reverse(HorizSpeed, BallX, BallRadius, WorldWidth);

            //определяем сопротивление, противоположное направлению + силу тяжести
            FallSpeed = DefineResistance(FallSpeed, airResist,BallX, BallY, Gravity);
            HorizSpeed = DefineResistance(HorizSpeed, airResist, BallX, BallY, 0);

            var mouse = Control.MousePosition;
            if (BallY >= (mouse.Y - BallRadius) - 5 && BallY <= (mouse.Y + BallRadius) + 5 &&
              BallX >= (mouse.X - BallRadius) - 5 && BallX <= (mouse.X + BallRadius) + 5)
            {
                if (BallY >= (mouse.Y - BallRadius) - 5 && BallY <= (mouse.Y + BallRadius) + 5)
                    FallSpeed = Math.Sin(FallSpeed);
                else if (BallX >= (mouse.X - BallRadius) - 5 && BallX <= (mouse.X + BallRadius) + 5)
                    HorizSpeed = Math.Cos(HorizSpeed);
            }

            //минимум для правой и нижней стен. макс - для верхней и левой
            BallY = PushOffTheWall(FallSpeed, BallY, BallRadius, WorldHeight, dt);
            BallX = PushOffTheWall(HorizSpeed, BallX, BallRadius, WorldWidth, dt);

            //Высчитывает очки при попадание мяча в область прямоугольников
            CountRedGoal = GoalCount(BallX <= WorldWidth * (partOccupByGate), CountRedGoal, BallX,  WorldWidth);
            CountGreenGoal = GoalCount(BallX >= WorldWidth * (1 - partOccupByGate), CountGreenGoal, BallX, WorldWidth);
        }

        static int GoalCount(bool crossedGate, int goals, double BallX, double WorldWidth)
        {
            if (CountRedGoal < countToWinGoals && !BallInField && crossedGate)
            {
                goals++;
                BallInField = true;
            }

            if (BallX > WorldWidth * (partOccupByGate) && BallX < WorldWidth * (1 - partOccupByGate))
            {
                BallInField = false;
            }

            return goals;
        }

       static double Reverse(double speed, double Ball, double BallRadius, double worldLen)
        {
            if (Ball == worldLen - BallRadius || Ball == BallRadius)
                speed = -speed;
            return speed;
        }

        static double PushOffTheWall(double speed, double Ball, double BallRadius, double worldCoord, double time)
        {
            return Math.Max(Math.Min(Ball + speed * time, worldCoord - BallRadius), BallRadius);
        }

        static double DefineResistance(double speed, double airResist, double BallX, double BallY, double Gravity)
        {
            var mouse = Control.MousePosition;
            double dx = mouse.X - BallX, dy = mouse.Y - BallY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            return speed >= 0 ? speed - airResist + Gravity - ResisCoefForDistance / distance : speed + airResist + Gravity + ResisCoefForDistance / distance;
        }
    }
}