using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Atividade
{
    public partial class frmAtividade : Form
    {
        public frmAtividade()
        {
            InitializeComponent();
        }



        private void btnRun_Click(object sender, EventArgs e)
        {
            if (txtArquivo.Text.Trim().Equals(""))
            {
                MessageBox.Show(this, "Caminho do arquivo deve ser informado");
                txtArquivo.Focus();
                return;
            }

            if (!File.Exists(txtArquivo.Text.Trim()))
            {
                MessageBox.Show(this, "Arquivo inexistente!");
                txtArquivo.Focus();
                return;
            }

            Thread thread = new Thread(() => ExecutaAtividade(txtArquivo.Text.Trim()));
            thread.Name = "Atividade - Run";
            thread.Start();
        }


        private void ExecutaAtividade(string filePath)
        {
            this.Invoke(new MethodInvoker(delegate()
            {
                txtArquivo.Enabled = false;
                btnRun.Enabled = false;
            }));

            try
            {
                CodigoAtividade(filePath);

                this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, "Finalizado!");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show(this, ex.Message);
                }));
            }
            finally
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    txtArquivo.Enabled = true;
                    btnRun.Enabled = true;
                }));
            }
        }
        
        private class Possibility{
            private int direction;
            private Boolean visited;
            
            public Possibility(int direction, Boolean visited){
                this.setDirection(direction);
                this.setVisisted(visited);
            }
            
            public int getDirection() => this.direction;
            public Boolean getVisited() => this.visited;
            public void setDirection(int direction) => this.direction = direction;
            public void setVisited(Boolean visited) => this.visited = visited;         
            
        }

        private class Crossroad{
            private int [] position;
            private int lastMove;
            private List<Possibility> possibilities;
            
            public Crossroad(int [] position){
                possibilities = new List<Possibility>();
                this.setPosition(position);

            }

            public void AddPossibilities(int p, Boolean v){
                Possibility possibility = new Possibility(p, v);
                possibilities.Add(possibility);
            }


            public int[] getPosition() => this.position;

            public int getLastMove() => this.lastMove;
            
            public List<int> getPossibilities() => this.possibilities;

            private void setPosition(int[] position) => this.position = position;

            private void setPossiblities(List<int> possibilities) => this.possibilities = possibilities;

            public void setLastMove(int lastMove) => this.lastMove = lastMove; 


        }

        private class Labrinth{

            private string filePath;
            private int [] startPoint;
            private int [] length;
            private int facing = -1;
            private string [,] maze;
            private ArrayList navigation;
            private List<Crossroad> crossroads;


            //constructor
            public Labrinth(string filePath){
                this.crossroads = new List<Crossroad>();
                LerArquivo(filePath);                
            }

            // void function to read the file with a file stream and separate the lines and treat them
            private void LerArquivo(string filePath){
                int count = 0;
                int count2 = 0;
                string [,] temp;
                // using file stream to read the file
                using (var stream = File.OpenRead(filePath)){
                    using (var reader = new StreamReader(stream)){  
                        this.setLength(reader.ReadLine());
                        temp = new string [this.length[0], this.length[1]];
                        string line;

                        while (!(String.IsNullOrEmpty(line = reader.ReadLine()))){
                            count2=0;
                            string[] aux = line.Split(' ');
                            foreach(string a in aux){
                                temp[count, count2] = a;
                                count2++;
                            }
                            count++;
                        }
                        
                        this.setMaze(temp);
                    }
                }
            }

            //function to locate the starting point of each maze
            private int[] StartPoint(){
                int [] aux = {-1,-1};
                for(int i = 0; i < this.length[0]; i++)
                    for(int j = 0; j < this.length[1]; j++)
                        if(this.maze[i,j] == "X"){
                            aux[0] = i;
                            aux[1] = j;
                            return aux;
                        }
                return aux;
            }

            private void LookArround(int[] position){
                
                //function to look arround and decide were to go from where you came
                Crossroad cross = new Crossroad(position);
                if(!this.getFacing().Equals(-1))
                    cross.setLastMove(this.facing);
                
                for (int i=0; i<4; i++)
                
                    switch(i){
                        case 0: 
                            if(position[0] - 1 >= 0){
                                if(this.maze[position[0]-1 , position[1]].Equals("0")){
                                    if(cross.getLastMove().Equals(3)){
                                        cross.AddPossibilities(i, true);
                                        break;
                                    } else {
                                        cross.AddPossibilities(i, false);
                                        break;
                                    }
                                } else break;
                            } else break;

                        case 1: 
                            if(position[1] - 1 >= 0){
                                if(this.maze[position[0], position[1]-1].Equals("0")){
                                    if(cross.getLastMove().Equals(2)){
                                        cross.AddPossibilities(i, true);
                                        break;
                                    } else {
                                        cross.AddPossibilities(i, false);
                                        break;
                                    }
                                } else break;
                            } else break;

                        case 2: 
                            if(position[1] + 1 < this.length[1]){
                                if(this.maze[position[0], position[1]+1].Equals("0")){
                                    if(cross.getLastMove().Equals(3)){
                                        cross.AddPossibilities(i, true);
                                        break;
                                    } else {
                                        cross.AddPossibilities(i, false);
                                        break;
                                    }
                                } else break;
                            } else break;

                        case 3: 
                            if(position[0] + 1 < this.length[0]){
                                if(this.maze[position[0]+1, position[1]].Equals("0")){
                                    if(cross.getLastMove().Equals(3)){
                                        cross.AddPossibilities(i, true);
                                        break;
                                    } else {
                                        cross.AddPossibilities(i, false);
                                        break;
                                    }
                                } else break;
                            } else break;

                        default:
                            break;
                        
                    }

                //if(cross.getPossibilities().Count >= 2)
                    this.crossroads.Add(cross);
            }

            // function that moves acording to a set priority defined at funtion LookArround
            private Boolean MoveFoward(int[] position){
                //validation to make sure we are at the same postition as it is stored in crossroads[].getPosition()
                if (this.crossroads[this.crossroads.Count-1].getPosition().Equals(position)){
                    // validation to make sure we have at least one possibility to move when in a crossroad
                    if(this.crossroads[this.crossroads.Count - 1].getPossibilities().Count > 0){
                        //setting facing var with the foremost possibility
                        this.setFacing(this.crossroads[this.crossroads.Count - 1].getPossibilities()[0]);
                        // using facing to determinate direction of movement
                        switch(this.getFacing()){
                        //in case x 
                            case 0:
                                position[0]--;
                                this.crossroads[this.crossroads.Count - 1].getPossibilities().RemoveAt(0);
                                return true;

                            case 1:
                                position[1]--;
                                this.crossroads[this.crossroads.Count - 1].getPossibilities().RemoveAt(0);
                                return true;

                            case 2:
                                position[1]++;
                                this.crossroads[this.crossroads.Count - 1].getPossibilities().RemoveAt(0);
                                return true;

                            case 3:
                                position[0]++;
                                this.crossroads[this.crossroads.Count - 1].getPossibilities().RemoveAt(0);
                                return true;

                            default:
                                return false;
                        }
                    } else return false;

                } else return false;

                return false;
            }


            private Boolean Move(int[] position){
                //facing =  0 is North
                //facing =  1 is West
                //facing =  2 is East
                //facing =  3 is South
                
                // checking if this is the first interaction
                if (!this.navigation.Count.Equals(0)){  
                    // checking if the actual position is the one already mapped
                    if(this.crossroads[this.crossroads.Count-1].getPosition().Equals(position)){  
                        //cheking if have more than 1 direction to go
                        if (this.crossroads[this.crossroads.Count() - 1].getPossibilities().Count().Equals(1))){ 
                            if(this.MoveFoward(position)){
                                this.crossroads.RemoveAt(this.crossroads.Count() - 1);
                                return true;
                            } else return false;
                        } else {            //this implies that we still have more than 1 route in this crossroad
                            //this checks if the most prioritized option is already visited and if not move to it
                            if(!this.crossroads[this.crossroads.Count() - 1].getPossibilities()[0].getVisited())
                                return this.MoveFoward(position);
                            else{ 
                                //this take the most prioritized option that have been visited and put it in the end of list of possibilities
                                // in that way even if the others are all dead ends, this path will be available to backtrack;
                                 Possibility aux = new Possibility();
                                 aux.setDirection(this.crossroads[this.crossroads.Count() - 1].getPossibilities()[0].getDirection());
                                 aux.setVisited(this.crossroads[this.crossroads.Count() - 1].getPossibilities()[0].getVisited());
                                 this.crossroads[this.crossroads.Count - 1].getPossibilities().RemoveAt(0);
                                 this.crossroads[this.crossroads.Count - 1].getPossibilities().Add(aux.getDirection(), aux.getVisited());
                                 return this.MoveFoward(position);
                            }
                        }
                     } else {      //this implies that this position possibilities haven't been mapped yet
                        LookArround(position);
                        //checking if we finished the maze
                        if(position[0] - 1 < 0 || position[1] - 1 < 0 || position[0] + 1 >= this.length[0] || position[1] + 1 >= this.length[1])
                            return false;
                        else    {
                            return this.MoveFoward(position);
                        }
                    }
                    return this.MoveFoward(position);
                } else {
                    //first movement
                    LookArround(position);
                    this.setFacing(this.crossroads[this.crossroads.Count].getPossibilities()[0]);
                    return this.MoveFoward(position);
                }
                
            }
            

            //function to navigate in the matrix using left hand algorithm to leave mazes.
            public void Navigate(){

                ArrayList route = new ArrayList();
                Boolean hasRoute = true;
                this.setStartPoint(this.StartPoint());
                int [] current = this.getStartPoint();

                // Adding the starting point on the result list
                string transition = "O ["+ (current[0]+1) + ", " + (current[1]+1) + "]";
                route.Add(transition); 

                while(hasRoute){
                    hasRoute = this.Move(current);
                    if(hasRoute)
                        switch (this.getFacing()){
                            case 0:
                                transition = "C ["+ (current[0]+1) + ", " + (current[1]+1) + "]";
                                route.Add(transition);
                                break;
                            case 1:
                                transition = "E ["+ (current[0]+1) + ", " + (current[1]+1) + "]";
                                route.Add(transition);
                                break;
                            case 2:
                                transition = "D ["+ (current[0]+1) + ", " + (current[1]+1) + "]";
                                route.Add(transition);
                                break;
                            case 3:
                                transition = "B ["+ (current[0]+1) + ", " + (current[1]+1) + "]";
                                route.Add(transition);
                                break;
                            default:
                                break;
                        }   
                }

                this.setNavigation(route);

            }

            //getters and setters
            public string getFilepath() => this.filePath;

            public int [] getLength() => this.length;
            
            public int [] getStartPoint() => this.startPoint;

            public string [,] getMaze() => this.maze;

            public ArrayList getNavigation() => this.navigation;
            
            public int getFacing() => this.facing;


            private void setFilePath(string filePath) => this.filePath = filePath;

            private void setLength(string length){
                //more elaborate setter that already do the parssing needed to separate the length
                string[] aux = length.Split(' ');
                this.length = aux.Select(int.Parse).ToArray();
            }

            
            private void setStartPoint(int [] startPoint) => this.startPoint = startPoint;

            private void setMaze(string [,] maze) => this.maze = maze;

            private void setNavigation(ArrayList navigation) => this.navigation = navigation;
            
            private void setFacing(int facing) => this.facing = facing;
        
        }


        private void CodigoAtividade(string filePath){

            Labrinth labrinth = new Labrinth(filePath);
            labrinth.Navigate();


        }


    }
}
