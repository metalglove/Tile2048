namespace Tile2048.ReinforcementLearning
{
    public class InterpretedGameState
    {
        public GameState GameState { get; private set; }
        public Direction Direction { get; private set; }

        public InterpretedGameState(GameState gameState, Direction direction)
        {
            GameState = gameState;
            Direction = direction;
        }
    }
}
