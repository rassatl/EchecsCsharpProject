using Commun;
using System.Windows.Forms;

namespace EchecsCsharpProject
{
    partial class ecranPrincipal : Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private TableLayoutPanel echiquierTable;
        private const int nbXYlenght = 8;
        private PictureBox selectedPictureBox = null;
        private Dictionary<string, object> savePiece = null;
        private List<String> listeCoupJouee = new List<string>();
        private ListBox listBoxCoupJouee;

        private void EcranPrincipal_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialisation de l'échiquier
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement de l'application : " + ex.Message);
            }
        }

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //SuspendLayout() et ResumeLayout() sont utilisés pour optimiser les performances lors de la modification dynamique de la disposition des contrôles dans un formulaire.
            SuspendLayout();
            //Ecran
            InitialiseWindow();
            //Echiquier
            InitializeBoard();
            InitialiseListBoxCoupJouee();
            ResumeLayout();
        }
        private void InitialiseListBoxCoupJouee()
        {
            // Initialiser la ListBox pour afficher la liste des coups joués
            listBoxCoupJouee = new ListBox();
            listBoxCoupJouee.Dock = DockStyle.Right;
            listBoxCoupJouee.Width = 500;
            Controls.Add(listBoxCoupJouee);
        }
        private void InitialiseWindow()
        {
            try
            {
                // 
                // ecranPrincipal
                // 
                AutoScaleDimensions = new SizeF(7F, 15F);
                AutoScaleMode = AutoScaleMode.Font;
                Screen screen = Screen.PrimaryScreen;
                ClientSize = new Size(screen.Bounds.Width * 3 / 4, screen.Bounds.Height * 3 / 4);
                MinimumSize = new Size(Constantes.WIDTH_MIN_SIZE, Constantes.HEIGHT_MIN_SIZE);
                Name = "Échiquier";
                Resize += new EventHandler(ecranPrincipal_Resize);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'initialisation de la fenêtre principale : " + ex.Message);
            }
        }

        // Création du damier de l'échiquier
        private void CreationGrid(TableLayoutPanel echiquierTab)
        {
            try
            {
                echiquierTable.ColumnCount = nbXYlenght;
                echiquierTable.Name = "echiquierTable";
                echiquierTable.RowCount = nbXYlenght;
                echiquierTable.TabIndex = 0;

                for (int i = 0; i < nbXYlenght; i++)
                {
                    echiquierTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / nbXYlenght));
                }
                for (int i = 0; i < nbXYlenght; i++)
                {
                    echiquierTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / nbXYlenght));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la création du damier de l'échiquier : " + ex.Message);
            }
        }
        private void InitializeDictionary(ref Dictionary<string, string> piecesTrie)
        {
            string[] pieces = null;

            try
            {
                //Récupération des pièces de l'échiquier
                pieces = Directory.GetFiles("Pieces");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la récupération des pièces : " + ex.Message);
            }

            if (pieces.Length > 0)
            {
                // trie des images par Noir ou Blanc suivant le nom de l'image
                for (int k = 0; k < pieces.Length; k++)
                {
                    if (pieces[k].Contains("Blanc"))
                        if (pieces[k].Contains("Roi"))
                            piecesTrie["RoiBlanc"] = pieces[k];
                        else if (pieces[k].Contains("Reine"))
                            piecesTrie["ReineBlanc"] = pieces[k];
                        else if (pieces[k].Contains("Fou"))
                            piecesTrie["FouBlanc"] = pieces[k];
                        else if (pieces[k].Contains("Cavalier"))
                            piecesTrie["CavalierBlanc"] = pieces[k];
                        else if (pieces[k].Contains("Tour"))
                            piecesTrie["TourBlanc"] = pieces[k];
                        else
                            piecesTrie["PionBlanc"] = pieces[k];
                    else
                        if (pieces[k].Contains("Roi"))
                        piecesTrie["RoiNoir"] = pieces[k];
                    else if (pieces[k].Contains("Reine"))
                        piecesTrie["ReineNoir"] = pieces[k];
                    else if (pieces[k].Contains("Fou"))
                        piecesTrie["FouNoir"] = pieces[k];
                    else if (pieces[k].Contains("Cavalier"))
                        piecesTrie["CavalierNoir"] = pieces[k];
                    else if (pieces[k].Contains("Tour"))
                        piecesTrie["TourNoir"] = pieces[k];
                    else if (pieces[k].Contains("Pion"))
                        piecesTrie["PionNoir"] = pieces[k];
                }               
            }
        }
        //Initialisation des pièces sur l'échiquier
        private void InitializePiecesPlacement(Dictionary<string, string> piecesTrie)
        {
            try
            {
                for (int i = 0; i < nbXYlenght; i++)
                {
                    for (int j = 0; j < nbXYlenght; j++)
                    {
                        //Création d'un panel pour chaque case de l'échiquier
                        Panel panel = new Panel();
                        panel.Dock = DockStyle.Fill;
                        panel.BackColor = (i + j) % 2 == 0 ? Color.White : Color.Gray;

                        Label label = new Label();
                        label.Text = GetSquareName(i, j);
                        label.TextAlign = ContentAlignment.BottomLeft;

                        //Ajout d'un PictureBox pour afficher les pièces
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Dock = DockStyle.Fill;
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                        //une fois les images triées, on les ajoute les pieces aux bonnes cases de l'échiquier
                        // pion (1, *), tour (0, 0), cavalier (0, 1), fou (0, 2), reine (0, 3), roi (0, 4), fou (0, 5), cavalier (0, 6), tour (0, 7)
                        // pion (6, *), tour (7, 0), cavalier (7, 1), fou (7, 2), reine (7, 3), roi (7, 4), fou (7, 5), cavalier (7, 6), tour (7, 7)
                        string pieceName = "";
                        if (j == 1)
                            pieceName = piecesTrie["PionNoir"];
                        else if (j == 0)
                            if (i == 0)
                                pieceName = piecesTrie["TourNoir"];
                            else if (i == 1)
                                pieceName = piecesTrie["CavalierNoir"];
                            else if (i == 2)
                                pieceName = piecesTrie["FouNoir"];
                            else if (i == 3)
                                pieceName = piecesTrie["ReineNoir"];
                            else if (i == 4)
                                pieceName = piecesTrie["RoiNoir"];
                            else if (i == 5)
                                pieceName = piecesTrie["FouNoir"];
                            else if (i == 6)
                                pieceName = piecesTrie["CavalierNoir"];
                            else
                                pieceName = piecesTrie["TourNoir"];
                        else if (j == 6)
                            pieceName = piecesTrie["PionBlanc"];
                        else if (j == 7)
                            if (i == 0)
                                pieceName = piecesTrie["TourBlanc"];
                            else if (i == 1)
                                pieceName = piecesTrie["CavalierBlanc"];
                            else if (i == 2)
                                pieceName = piecesTrie["FouBlanc"];
                            else if (i == 3)
                                pieceName = piecesTrie["ReineBlanc"];
                            else if (i == 4)
                                pieceName = piecesTrie["RoiBlanc"];
                            else if (i == 5)
                                pieceName = piecesTrie["FouBlanc"];
                            else if (i == 6)
                                pieceName = piecesTrie["CavalierBlanc"];
                            else
                                pieceName = piecesTrie["TourBlanc"];
                        if (pieceName != "")
                            pictureBox.Image = Image.FromFile(pieceName);
                        object data = new object();
                        Dictionary<string, object> datas = new Dictionary<string, object>()
                        {
                            { "Key", GetKeyFromValue(piecesTrie, pieceName) },
                            { "PositionX", i },
                            { "PositionY", j },
                            { "ImageLink", pieceName }
                        };

                        pictureBox.Tag = datas;
                        pictureBox.Click += new EventHandler(PictureBox_Click);

                        panel.Controls.Add(label);
                        panel.Controls.Add(pictureBox);
                        echiquierTable.Controls.Add(panel, i, j);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'initialisation des pièces sur l'échiquier : " + ex.Message);
            }
        }

        private bool IsMoveValid(int sourceX, int sourceY, int destinationX, int destinationY)
        {
            // Par exemple, vérifiez si le déplacement est d'une case en avant
            return true; //destinationY == sourceY - 1 && destinationX == sourceX;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            // Quand on clique sur une pièce, on récupère le nom de l'image et on peut la déplacer d'une case en avant et si on clique sur une case où il n'y a pas de pièce, on ne fait rien
            PictureBox clickedPictureBox = (PictureBox)sender;
            Dictionary<string, object> pieceData = (Dictionary<string, object>)clickedPictureBox.Tag;

            //Piece sélectionnée ?
            if (selectedPictureBox == null && savePiece == null)
            {
                // Aucune pièce sélectionnée, vérifier si la case cliquée contient une pièce
                if ((String)pieceData.Values.ElementAt(0) != "CaseVide")
                {
                    // Sélectionner la pièce
                    selectedPictureBox = clickedPictureBox;
                    savePiece = pieceData; // Key, PositionX, PositionY
                }
            }
            else
            {
                // Une pièce est déjà sélectionnée, vérifier si le déplacement est valide
                int sourceX = (int)savePiece.Values.ElementAt(1);
                int sourceY = (int)savePiece.Values.ElementAt(2);
                int destinationX = (int)pieceData.Values.ElementAt(1);
                int destinationY = (int)pieceData.Values.ElementAt(2);

                // Vérifier si le déplacement est valide (par exemple, déplacement d'une case en avant)
                if (IsMoveValid(sourceX, sourceY, destinationX, destinationY))
                {
                    // Effectuer le déplacement de la pièce
                    DeplacementPiece(selectedPictureBox, destinationX, destinationY);

                    // Réinitialiser la pièce sélectionnée
                    selectedPictureBox = null;
                    savePiece = null;
                }
                else
                {
                    MessageBox.Show("Déplacement invalide !");
                }
                selectedPictureBox = null;
            }
        }

        private void DeplacementPiece(PictureBox sourcePictureBox, int destinationX, int destinationY)
        {
            // Récupérer les données de la pièce
            Dictionary<string, object> pieceData = (Dictionary<string, object>)sourcePictureBox.Tag;

            // Récupérer le Panel parent du PictureBox
            Panel sourcePanel = (Panel)sourcePictureBox.Parent;

            // Supprimer le PictureBox du Panel actuel
            sourcePanel.Controls.Remove(sourcePictureBox);

            // Obtenir le Panel de destination à partir du TableLayoutPanel
            Control destinationControl = echiquierTable.GetControlFromPosition(destinationX, destinationY);

            // Vérifier si le contrôle de destination est un Panel
            if (destinationControl is Panel destinationPanel)
            {
                // Créer un nouveau PictureBox pour la pièce déplacée
                PictureBox destinationPictureBox = new PictureBox();
                destinationPictureBox.Dock = DockStyle.Fill;
                destinationPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                destinationPictureBox.Image = Image.FromFile((string)pieceData["ImageLink"]);
                destinationPictureBox.Click += new EventHandler(PictureBox_Click);
                // Créer un nouvel objet de données pour le nouveau PictureBox
                Dictionary<string, object> destinationPieceData = new Dictionary<string, object>()
                {
                    { "Key", pieceData["Key"] },
                    { "PositionX", destinationX },
                    { "PositionY", destinationY },
                    { "ImageLink", pieceData["ImageLink"] }
                };
                destinationPictureBox.Tag = destinationPieceData;

                // Ajouter le nouveau PictureBox au Panel de la case de destination
                destinationPanel.Controls.Remove(destinationPanel.Controls[1]);
                destinationPanel.Controls.Add(destinationPictureBox);

                // Afficher un message indiquant le déplacement
                string destinationSquare = GetSquareName(destinationX, destinationY);
                listeCoupJouee.Add("La pièce " + (string)pieceData["Key"] + " a été déplacée vers la case " + destinationSquare + ".");
                // Mettre à jour la liste des coups joués dans la ListBox
                listBoxCoupJouee.Items.Clear();
                listBoxCoupJouee.Items.AddRange(listeCoupJouee.ToArray());
            }
            else
            {
                // Afficher un message d'erreur si le contrôle de destination n'est pas un Panel
                MessageBox.Show("Erreur : Le contrôle de destination n'est pas un Panel.");
            }
        }




        //Récupérer le nom de la case à partir des coordonnées
        private string GetSquareName(int x, int y)
        {
            // Convertir les coordonnées (x, y) en nom de case (ex: "A1", "E4", etc.)
            char file = (char)('A' + x);
            int rank = 8 - y;
            return $"{file}{rank}";
        }

        private string GetKeyFromValue(Dictionary<string, string> dictionary, string value)
        {
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (pair.Value == value)
                {
                    return pair.Key;
                }
            }
            return "CaseVide";
        }

        private void InitializeBoard()
        {
            try
            {
                echiquierTable = new TableLayoutPanel();

                CreationGrid(echiquierTable);

                Dictionary<string, string> piecesTrie = new Dictionary<string, string>()
                    {
                        { "RoiNoir", "" },
                        { "ReineNoir", "" },
                        { "FouNoir", "" },
                        { "CavalierNoir", "" },
                        { "TourNoir", "" },
                        { "PionNoir", "" },
                        { "RoiBlanc", "" },
                        { "ReineBlanc", "" },
                        { "FouBlanc", "" },
                        { "CavalierBlanc", "" },
                        { "TourBlanc", "" },
                        { "PionBlanc", "" }
                    };
                InitializeDictionary(ref piecesTrie);

                InitializePiecesPlacement(piecesTrie);

                Controls.Add(echiquierTable);
                ResizeEchiquier();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'initialisation de l'échiquier : " + ex.Message);
            }
        }

        private void ecranPrincipal_Resize(object sender, EventArgs e)
        {
            ResizeEchiquier();
        }

        private void ResizeEchiquier()
        {
            if (echiquierTable != null)
            {
                int newSize = Math.Max(Constantes.WIDTH_MIN_SIZE, Math.Min(ClientSize.Width, ClientSize.Height));
                echiquierTable.Size = new Size(newSize, newSize);
            }
        }

        private System.ComponentModel.IContainer components = null;
        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
