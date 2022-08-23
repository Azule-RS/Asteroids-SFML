using Engine;

namespace Asteroids
{
    public class Program
    {
        public static void Main(string[] parameters)
        {
            // Engine initialization
            GameEngine engine = new GameEngine("Asteroids");

            // Creating a node that controls the main actions in the game.
            Node managerNode = new Node("GameManager");
            GameManager manager = managerNode.AddComponent<GameManager>();

            // Create a node representing the player.
            Node player = new Node("Player");
            manager.Player = player.AddComponent<Player>();
            manager.LoopNodes.Add(player);

            // Spawn of asteroids
            for (int i = 0; i < 4; i++)
            {
                Node asteroid = new Node($"Asteroid_{i}");
                manager.LoopNodes.Add(asteroid);
                manager.Asteroids.Add(asteroid.AddComponent<Asteroid>());
            }
        }
    }
}