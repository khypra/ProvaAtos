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

        private class Crossroad{
            private int [] position;
            private int lastMove;
            private List<int> possibilities;
            
            public Crossroad(int [] position){
                possibilities = new List<int>();
                this.setPosition(position);

            }

            public void AddPossibilities(int p){
                possibilities.Add(p);
            }


            public int[] getPosition() => this.position;

            public int getLastMove() => this.lastMove;
            
            public List<int> getPossibilities() => this.possibilities;

            private void setPosition(int[] position) => this.position = position;

            private void setPossiblities(List<int> possibilities) => this.possibilities = possibilities;

            public void setLastMove(int lastMove) => this.lastMove = lastMove; 


        }

        private class Labrinth{

            string filePath;
            int [] startPoint;
            int [] length;
            int facing = -1;
            string [,] maze;
            ArrayList navigation;
            List<int> history;
            List<Crossroad> crossroads;


            //constructor
            public Labrinth(string filePath){
                this.crossroads = new List<Crossroad>();
                this.history = new List<int>();
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
                                        cross.AddPossibilities(i);
                                        break;
                                } else break;
                            } else break;

                        case 1: 
                            if(position[1] - 1 >= 0){
                                if(this.maze[position[0], position[1]-1].Equals("0")){
                                        cross.AddPossibilities(i);
                                        break;
                                } else break;
                            } else break;

                        case 2: 
                            if(position[1] + 1 < this.length[1]){
                                if(this.maze[position[0], position[1]+1].Equals("0")){
                                        cross.AddPossibilities(i);
                                        break;
                                } else break;
                            } else break;

                        case 3: 
                            if(position[0] + 1 < this.length[0]){
                                if(this.maze[position[0]+1, position[1]].Equals("0")){
                                        cross.AddPossibilities(i);
                                        break;
                                } else break;
                            } else break;

                        default:
                            break;
                        
                    }

                //if(cross.getPossibilities().Count >= 2)
                    this.crossroads.Add(cross);
            }

            

            private Boolean MoveFoward(int[] position){

                if (this.crossroads[this.crossroads.Count-1].getPosition().Equals(position)){
                    if(this.crossroads[this.crossroads.Count - 1].getPossibilities().Count > 0){
                        this.setFacing(this.crossroads[this.crossroads.Count - 1].getPossibilities()[0]);
                        switch(this.getFacing()){
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
                //switch case to change where you are facing and move towards that direction one block
                
                Boolean control = false;
                LookArround(position);

                if (!this.history.Count.Equals(0)){
                    return this.MoveFoward(position);
                } else {
                    //first movement
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
                        switch (this.facing){
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
