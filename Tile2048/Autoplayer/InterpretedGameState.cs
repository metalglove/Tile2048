namespace Tile2048.Autoplayer
{
    public class InterpretedGameState
    {
        public GameState GameState { get; private set; }
        public Direction Direction { get; set; }
        public int Depth { get; set; } = 0;
        public InterpretedGameState(GameState gameState, Direction direction)
        {
            GameState = gameState;
            Direction = direction;
        }
    }
}
