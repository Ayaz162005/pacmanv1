using Pacman.Classes;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace Pacman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PacmanClass pacman;
        private PacmanClass pacmanEnemyRed;

        private DispatcherTimer timer;
       
        private double moveSpeed = 20;
        public MainWindow()
        {
            InitializeComponent();
            DrawMap();
            InitializePacman();
            InitializeEnemyPacmanRed();
            InitializeTimer();
            KeyDown += MainWindow_KeyDown;
        }

        private void DrawMap()
        {
            // Duvarları çiz
            for (int y = 0; y < 17; y++)
            {
                for (int x = 0; x < 58; x++)
                {
                    bool hasLeftNeighbor = (x > 0 && Map[y][x - 1] == '#');
                    bool hasRightNeighbor = (x < 57 && Map[y][x + 1] == '#');
                    bool hasTopNeighbor = (y > 0 && Map[y - 1][x] == '#');
                    bool hasBottomNeighbor = (y < 16 && Map[y + 1][x] == '#');

                    if (Map[y][x] == '#')
                    {
                      
                        Border wall = new Border();
                        wall.Width = 20;
                        wall.Height = 20;

                        wall.BorderBrush = Brushes.Blue; // Set border color
                        wall.BorderThickness = new Thickness(hasLeftNeighbor ? 0 : 1, hasTopNeighbor ? 0 : 1, hasRightNeighbor? 0:1, hasBottomNeighbor ? 0:1);

                        wall.CornerRadius = new CornerRadius(!hasLeftNeighbor&&!hasTopNeighbor ? 10:0,!hasTopNeighbor && !hasRightNeighbor ? 10: 0,!hasRightNeighbor && !hasBottomNeighbor ? 10: 0,!hasBottomNeighbor &&  !hasLeftNeighbor? 10:0);
                        Canvas.SetLeft(wall, x * 20);
                        Canvas.SetTop(wall, y * 20);
                        canvas.Children.Add(wall);
                    }
                    if (Map[y][x] == '.')
                    {
                        Ellipse dot = new Ellipse();
                        dot.Width = 5;
                        dot.Height = 5;
                        dot.Fill = Brushes.White;

                        // Calculate the center position of the cell
                        double centerX = (x * 20) + 10; // Half of the cell width
                        double centerY = (y * 20) + 10; // Half of the cell height

                        // Set the center position of the dot
                        Canvas.SetLeft(dot, centerX - (dot.Width / 2)); // Adjusting to the center
                        Canvas.SetTop(dot, centerY - (dot.Height / 2)); // Adjusting to the center

                        canvas.Children.Add(dot);
                    }
                    if (Map[y][x] == '@')
                    {
                        Border wall = new Border();
                        wall.Width = 20;
                        wall.Height = 20;



                        wall.CornerRadius = new CornerRadius(!hasLeftNeighbor && !hasTopNeighbor ? 5 : 0, 0, 0, 0);
                        Canvas.SetLeft(wall, x * 20);
                        Canvas.SetTop(wall, y * 20);
                        canvas.Children.Add(wall);
                    }
                }
            }
        }

        // Google Pac-Man haritası
        private readonly string[] Map = new string[]
        {
       "##########################################################",
       "#........................................................#",
       "#.###.#########.###############.########@###.#####.#####.#",
       "#.###.#########.###############.########@###.#####.#####.#",
       "#.....#########..................@@@@@@@@###.............#",
       "#####.###.......#######.#######@########@###.#######.#####",
       "@@@@#.###.#####.#######.#######@@@@@@@@@@###.###@###.#@@@@",
       "#####.###.#####.###@###.###@###@@@@@@@@@@###.###@###.#####",
       "@@@@@.###...###.###@###.###@###@@@@@@@@@@###.#######.@@@@@",
       "#####.#########.#######.#######@@@@@@@@@@###.###.....#####",
       "@@@@#.#########.#######.#######@@@@@@@@@@###.#######.#@@@@",
       "#####.#########.#######.#######@@@@@@@@@@###.#######.#####",
       "#...............................########.................#",
       "#.####.##########.##.##########....##....#########.#####.#",
       "#.####.##########.##.##########.##.##.##.#########.#####.#",
       "#.................##............##....##.................#",
       "##########################################################"
        };



        private void InitializePacman()
        {
            pacman = new PacmanClass();
            pacman.PositionX = 20; // Initial X position
            pacman.PositionY = 20; // Initial Y position
            pacman.Direction = Direction.Right;
            pacman.NextDirection = Direction.Right;
            // Initial direction
            DrawPacman();
        }
        private void InitializeEnemyPacmanRed()
        {
            pacmanEnemyRed = new PacmanClass();
            pacmanEnemyRed.PositionX = 660; // Initial X position
            pacmanEnemyRed.PositionY = 200; // Initial Y position
            pacmanEnemyRed.Direction = Direction.Right;
            pacmanEnemyRed.NextDirection = Direction.Right;
            // Initial direction
            DrawEnemyPacmanRed();
        }

        private void DrawEnemyPacmanRed()
        {
            Ellipse pacmanShape = new Ellipse();
            pacmanShape.Width = 20;
            pacmanShape.Height = 20;
            pacmanShape.Fill = Brushes.Red;

            // Interpolate Pac-Man's position between grid cells

            Canvas.SetLeft(pacmanShape, pacmanEnemyRed.PositionX);
            Canvas.SetTop(pacmanShape, pacmanEnemyRed.PositionY);

            canvas.Children.Add(pacmanShape);
        }


        private void DrawPacman()
        {
            Ellipse pacmanShape = new Ellipse();
            pacmanShape.Width = 20;
            pacmanShape.Height = 20;
            pacmanShape.Fill = Brushes.Yellow;

            // Interpolate Pac-Man's position between grid cells
           
            Canvas.SetLeft(pacmanShape, pacman.PositionX);
            Canvas.SetTop(pacmanShape, pacman.PositionY);

            canvas.Children.Add(pacmanShape);
        }

        private void MovePacman()
        {
            double nextX = pacman.PositionX;
            double nextY = pacman.PositionY;
            double nextX1 = pacman.PositionX;
            double nextY1 = pacman.PositionY;

            // Calculate next position based on direction
            switch (pacman.Direction)
            {
                case Direction.Up:
                    nextY -= moveSpeed % 100; // Smoothly move Pac-Man upwards
                    break;
                case Direction.Down:
                    nextY += moveSpeed % 100; // Smoothly move Pac-Man downwards
                    break;
                case Direction.Left:
                    nextX -= moveSpeed % 100; // Smoothly move Pac-Man to the left
                    break;
                case Direction.Right:
                    nextX += moveSpeed % 100; // Smoothly move Pac-Man to the right
                    break;
            }
            switch (pacman.NextDirection)
            {
                case Direction.Up:
                    nextY1 -= moveSpeed % 100; // Smoothly move
                    break;
                    case Direction.Down:
                    nextY1 += moveSpeed % 100; // Smoothly move
                    break;
                    case Direction.Left:
                    nextX1 -= moveSpeed % 100; // Smoothly move
                    break;
                    case Direction.Right:
                    nextX1 += moveSpeed % 100; // Smoothly move
                    break;
                    default:
                    break;

            }
            if (nextY1 == 160 && nextX1 == -20)
            {
                //alert("Game Over");

                pacman.PositionY = 160;
                pacman.PositionX = 1140;
                pacman.Direction = Direction.Left;
                pacman.NextDirection = Direction.Left;
                canvas.Children.Clear();
                DrawMap();
                DrawPacman();
                DrawEnemyPacmanRed();

            }
            if (nextY1 == 160 && nextX1 == 1160)
            {
                //alert("Game Over");

                pacman.PositionY = 160;
                pacman.PositionX = 20;
                pacman.Direction = Direction.Right;
                pacman.NextDirection = Direction.Right;
                canvas.Children.Clear();
                DrawMap();
                DrawPacman();
                DrawEnemyPacmanRed();

            }

            // Check if the next position is within the boundaries of the canvas
            if (nextX >= 0 && nextX < canvas.ActualWidth && nextY >= 0 && nextY < canvas.ActualHeight)
            {
                // Check if the next position is not a wall
                int gridX = (int)(nextX / 20); // Convert next X position to grid coordinates
                int gridY = (int)(nextY / 20); // Convert next Y position to grid coordinates
                int gridX1 = (int)(nextX1 / 20); // Convert next X position to grid coordinates
                int gridY1 = (int)(nextY1 / 20);
                // Convert next Y position to grid coordinates





                if (gridX1 < 58 && gridX < 58)
                {

                    if (Map[gridY1][gridX1] != '#')
                    {
                        // Update Pac-Man's position
                        if (Map[gridY1][gridX1] == '.')
                        {
                            Map[gridY1] = Map[gridY1].Remove(gridX1, 1).Insert(gridX1, "@");
                            canvas.Children.Clear();
                            DrawMap();
                            DrawPacman();
                                DrawEnemyPacmanRed();


                        }

                        pacman.PositionX = nextX1;
                        pacman.PositionY = nextY1;
                        pacman.Direction = pacman.NextDirection;
                        // Redraw Pac-Man
                        canvas.Children.Clear();
                        DrawMap();
                        DrawPacman();
                        DrawEnemyPacmanRed();
                    }


                    else if (Map[gridY][gridX] != '#')
                    {
                        if (Map[gridY][gridX] == '.')
                        {
                            Map[gridY] = Map[gridY].Remove(gridX, 1).Insert(gridX, "@");
                            canvas.Children.Clear();
                            DrawMap();
                            DrawPacman();
                            DrawEnemyPacmanRed();


                        }
                        // Update Pac-Man's position
                        pacman.PositionX = nextX;
                        pacman.PositionY = nextY;

                        // Redraw Pac-Man
                        canvas.Children.Clear();
                        DrawMap();
                        DrawPacman();
                        DrawEnemyPacmanRed();
                    }
                }
               
            }
        
        }

        private void MoveEnemyPacmanRed()
        {
            // Calculate the next position based on the current direction
            double nextX = pacmanEnemyRed.PositionX;
            double nextY = pacmanEnemyRed.PositionY;

            // Determine the target position (Pac-Man's position)
            double targetX = pacman.PositionX;
            double targetY = pacman.PositionY;

            double distanceX = targetX - nextX;
            double distanceY = targetY - nextY;

            if (distanceX == 0 && distanceY == 0)
            {
                // Game over
                //alert("Game Over");
                //alert
                MessageBox.Show("Game Over");
           

               
            }
         

            Direction nowDirection = pacmanEnemyRed.Direction;

         
            if (Math.Abs(distanceX) < Math.Abs(distanceY))
            {
                pacmanEnemyRed.NextDirection = distanceX > 0 ? Direction.Right : Direction.Left;
            }
            else if (Math.Abs(distanceY) < Math.Abs(distanceX))
            {
                pacmanEnemyRed.NextDirection = distanceY > 0 ? Direction.Down : Direction.Up;
            }
            //else
            //{
            //    pacmanEnemyRed.NextDirection = pacmanEnemyRed.Direction;
            //    //pacmanEnemyRed.NextDirection = distanceY > 0 ? Direction.Down : Direction.Up;
            //}

            if (distanceX == 0)
            {
                pacmanEnemyRed.NextDirection = distanceY > 0 ? Direction.Down : Direction.Up;



            }
            if (distanceY == 0)
            {
                
                pacmanEnemyRed.NextDirection = distanceX > 0 ? Direction.Right : Direction.Left;

            }



            if (pacmanEnemyRed.Direction ==Direction.Right && !(pacmanEnemyRed.Direction == Direction.Right && pacmanEnemyRed.NextDirection == Direction.Left))
            {
                pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;
            }
            else if (pacmanEnemyRed.Direction == Direction.Left && !(pacmanEnemyRed.Direction == Direction.Left && pacmanEnemyRed.NextDirection == Direction.Right))
            {
                pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;
            }
            else if ( pacmanEnemyRed.Direction == Direction.Up && !(pacmanEnemyRed.Direction == Direction.Up && pacmanEnemyRed.NextDirection == Direction.Down))
            {
                pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;
            }
            else if ( pacmanEnemyRed.Direction == Direction.Down && !(pacmanEnemyRed.Direction == Direction.Down && pacmanEnemyRed.NextDirection == Direction.Up))
            {
                pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;
            }

            


            switch (pacmanEnemyRed.Direction)
            {
                case Direction.Up:
                    nextY -= moveSpeed % 100; // Smoothly move
                    break;
                case Direction.Down:
                    nextY += moveSpeed % 100; // Smoothly move
                    break;
                case Direction.Left:
                    nextX -= moveSpeed % 100; // Smoothly move
                    break;
                case Direction.Right:
                    nextX += moveSpeed % 100; // Smoothly move
                    break;
                default:
                    break;
            }

            // Check if the next position is within the boundaries of the canvas
            if (nextX >= 0 && nextX < canvas.ActualWidth && nextY >= 0 && nextY < canvas.ActualHeight)
            {
                // Check if the next position is not a wall
                int gridX = (int)(nextX / 20); // Convert next X position to grid coordinates
                int gridY = (int)(nextY / 20); // Convert next Y position to grid coordinates

                if (gridX < 58)
                {
                    if (Map[gridY][gridX] != '#')
                    {
                        pacmanEnemyRed.PositionX = nextX;
                        pacmanEnemyRed.PositionY = nextY;
                       
                        // Redraw Pac-Man
                        canvas.Children.Clear();
                        DrawMap();
                        DrawPacman();
                        DrawEnemyPacmanRed();

                    }
                    else
                    {
          



                        if (pacmanEnemyRed.Direction == Direction.Up || pacmanEnemyRed.Direction == Direction.Down)
                        {
                            if (distanceX > 0)
                            {
                               if(nowDirection != Direction.Left)
                      
                                pacmanEnemyRed.NextDirection = Direction.Right;
                            }
                            else if (distanceX < 0)
                            {
                                if (nowDirection != Direction.Right)
                                pacmanEnemyRed.NextDirection = Direction.Left;
                            }
                            else
                            {
                                if (pacmanEnemyRed.Direction == Direction.Up)
                                {

                                    if (nowDirection != Direction.Up)
                                    {
                                        pacmanEnemyRed.NextDirection = Direction.Down;



                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);

                                        if (Map[gridy3][gridx3 - 1] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }

                                        else if (Map[gridy3 + 1][gridx3 + 1] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }
                                    }
                                    else
                                    {
                                        //check next location is wall or not

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);
                                        if (Map[gridy3][gridx3+1] != '#' && Map[gridy3][gridx3-1] != '#')
                                        {
                                            if (pacman.PositionX  >  640)
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Right;
                                            }
                                            else
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Left;

                                            }
                                        }
                                        else
                                        {
                                            pacmanEnemyRed.NextDirection = Map[gridy3][gridx3 +1] == '#' ? Direction.Right : Direction.Left;
                                        }

                                    }
                                }
                                else
                                {
                                    if (nowDirection != Direction.Down)
                                    {
                                        pacmanEnemyRed.NextDirection = Direction.Up;

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);

                                        if (Map[gridy3][gridx3+1] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }

                                        else if (Map[gridy3 + 1][gridx3 + 1] == '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }
                                    }
                                    else
                                    {
                                        //check next location is wall or not

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);
                                        if (Map[gridy3][gridx3 +1] != '#' && Map[gridy3][gridx3 -1] != '#')
                                        {
                                            if (pacman.PositionY > 640)
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Right;
                                            }
                                            else
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Left;

                                            }
                                        }
                                        else
                                        {
                                            pacmanEnemyRed.NextDirection = Map[gridy3][gridx3 -1] == '#' ? Direction.Right : Direction.Left;
                                        }



                                    }

                                }

                            }
                        }
                        else if (pacmanEnemyRed.Direction == Direction.Left || pacmanEnemyRed.Direction == Direction.Right)
                        {
                            if (distanceY > 0)
                            {
                                if (nowDirection != Direction.Up)
                                pacmanEnemyRed.NextDirection = Direction.Down;
                            }
                            else if(distanceY < 0)
                            {
                                if (nowDirection != Direction.Down)
                                pacmanEnemyRed.NextDirection = Direction.Up;
                            }
                            else
                            {
                                if(pacmanEnemyRed.Direction == Direction.Left)
                                {
                                    if (nowDirection != Direction.Left)
                                    {
                                        pacmanEnemyRed.NextDirection = Direction.Right;



                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);

                                        if (Map[gridy3 + 1][gridx3] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }

                                      else if(Map[gridy3 + 1][gridx3+1] == '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }
                                    }
                                    else
                                    {
                                        //check next location is wall or not

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);
                                        if (Map[gridy3 + 1 ][gridx3] != '#' && Map[gridy3 -1][gridx3] != '#')
                                        {
                                            if (pacman.PositionY > 320)
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Down;
                                            }
                                            else
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Up;

                                            }
                                        }
                                        else
                                        {
                                            pacmanEnemyRed.NextDirection = Map[gridy3 + 1][gridx3] == '#' ? Direction.Up : Direction.Down;
                                        }

                                    }
                                }
                                else
                                {
                                    if (nowDirection != Direction.Right)
                                    {
                                        pacmanEnemyRed.NextDirection = Direction.Left;

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);

                                        if (Map[gridy3 + 1][gridx3] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }

                                        else if (Map[gridy3 + 1][gridx3 + 1] != '#')
                                        {
                                            pacmanEnemyRed.NextDirection = nowDirection;

                                        }
                                    }
                                    else
                                    {
                                        //check next location is wall or not

                                        int gridx3 = (int)(pacmanEnemyRed.PositionX / 20);
                                        int gridy3 = (int)(pacmanEnemyRed.PositionY / 20);
                                        if (Map[gridy3 + 1][gridx3] != '#' && Map[gridy3 - 1][gridx3] != '#')
                                        {
                                            if (pacman.PositionY > 320)
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Down;
                                            }
                                            else
                                            {
                                                pacmanEnemyRed.NextDirection = Direction.Up;

                                            }
                                        }
                                        else
                                        {
                                            pacmanEnemyRed.NextDirection = Map[gridy3 + 1][gridx3] == '#' ? Direction.Up : Direction.Down;
                                        }
                                   
                                    

                                    }

                                }
                            
                            }
                        }

                        if(pacmanEnemyRed.Direction == pacmanEnemyRed.NextDirection)
                        {
                            pacmanEnemyRed.NextDirection = nowDirection;
                        }

                        pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;



                        double nextY2 = pacmanEnemyRed.PositionY;
                        double nextX2 = pacmanEnemyRed.PositionX;
                        switch (pacmanEnemyRed.Direction)
                        {
                            case Direction.Up:
                                nextY2 -= moveSpeed % 100; // Smoothly move
                                break;
                            case Direction.Down:
                                nextY2 += moveSpeed % 100; // Smoothly move
                                break;
                            case Direction.Left:
                                nextX2 -= moveSpeed % 100; // Smoothly move
                                break;
                            case Direction.Right:
                                nextX2 += moveSpeed % 100; // Smoothly move
                                break;
                            default:
                                break;
                        }
                        pacmanEnemyRed.PositionX = nextX2;
                        pacmanEnemyRed.PositionY = nextY2;
                        pacmanEnemyRed.Direction = pacmanEnemyRed.NextDirection;
                        // Redraw Pac-Man
                        canvas.Children.Clear();
                        DrawMap();
                        DrawPacman();
                        DrawEnemyPacmanRed();


                    }
                }
            }





            

            // Redraw the red ghost on the canvas
            canvas.Children.Clear();
            DrawMap();
            DrawPacman();
            DrawEnemyPacmanRed();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
           
            // Set the interval for Pac-Man's movement
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            
            MovePacman();
            MoveEnemyPacmanRed();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            

            // Change Pac-Man's direction based on keyboard input
            switch (e.Key)
            {
                case Key.Up:
                  
                    pacman.NextDirection = Direction.Up;
                    break;
                case Key.Down:
                    
                    pacman.NextDirection = Direction.Down;
                    break;
                case Key.Left:
                  
                    pacman.NextDirection = Direction.Left;
                    break;
                case Key.Right:
                    pacman.NextDirection = Direction.Right;
                    break;

                
            }

         
            double nextX = pacman.PositionX;
            double nextY = pacman.PositionY;
            switch (pacman.NextDirection)
            {
                case Direction.Up:
                    nextY -= moveSpeed % 100; // Smoothly move Pac-Man upwards
                    break;
                case Direction.Down:
                    nextY += moveSpeed % 100; // Smoothly move Pac-Man downwards
                    break;
                case Direction.Left:
                    nextX -= moveSpeed % 100; // Smoothly move Pac-Man to the left
                    break;
                case Direction.Right:
                    nextX += moveSpeed % 100; // Smoothly move Pac-Man to the right
                    break;
            }
          

            if (nextX >= 0 && nextX < canvas.ActualWidth && nextY >= 0 && nextY < canvas.ActualHeight)
            {
                // Check if the next position is not a wall
                int gridX = (int)(nextX / 20); // Convert next X position to grid coordinates
                int gridY = (int)(nextY / 20); // Convert next Y position to grid coordinates

                if(gridX < 58)
                {

                
                if (Map[gridY][gridX] != '#')
                {
                     pacman.Direction = pacman.NextDirection;
                }
                
                }
                else if(gridX == 58)
                {
                   pacman.Direction = pacman.NextDirection;
                }
               
                
              
            }

            //MovePacman();
        }
    }
}
//56
//48