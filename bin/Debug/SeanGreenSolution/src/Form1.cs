using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DepositionMaterial
{
    public partial class Form1 : Form
    {
        //Variables set in the interface
        float lowerRectangleWidth, radius, lowerRectangleLength;
        float upperRectangleWidth, upperRectangleLength, stepUpHeight, height;
        string pathDirection;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {       

            try
            {
                //When submit button is clicked grab the parameters
                upperRectangleWidth = float.Parse(textBox1.Text, CultureInfo.InvariantCulture.NumberFormat);
                upperRectangleLength = float.Parse(textBox2.Text, CultureInfo.InvariantCulture.NumberFormat);
                lowerRectangleWidth = float.Parse(textBox3.Text, CultureInfo.InvariantCulture.NumberFormat);
                lowerRectangleLength = float.Parse(textBox4.Text, CultureInfo.InvariantCulture.NumberFormat);
                height = float.Parse(textBox5.Text, CultureInfo.InvariantCulture.NumberFormat);
                stepUpHeight = float.Parse(textBox6.Text, CultureInfo.InvariantCulture.NumberFormat);
                radius = float.Parse(textBox7.Text, CultureInfo.InvariantCulture.NumberFormat);
                pathDirection = comboBox1.Text;
            }

            catch
            {
                MessageBox.Show("All textboxes must have values");
                return;
            }

            //Make sure step height is divisor of part height. If it is not, don't generate the path coordinates
            if (height % stepUpHeight != 0)
            {
                MessageBox.Show("Step Up Height must be divisor of Overall Part Height. Please enter different data.");
                return;
            }

            //Make sure circle can fit in the rectangle
            if ((radius*2) > lowerRectangleLength || (radius*2) > lowerRectangleWidth || (radius*2) > upperRectangleLength || (radius*2) > upperRectangleWidth)
            {
                MessageBox.Show("Cylinder cannot fit inside rectangle. Please enter different data.");
                return;
            }

            generatePath();
         
        }

        private void generatePath()
        {
            Coordinate point = new DepositionMaterial.Coordinate();           
            float remainderWidth, remainderLength;       
            int nCirclesWidth, nCirclesLength;
            int nSteps;
            List<Coordinate> coordinates = new List<Coordinate>();
            List<Rectangle> rectangles = new List<Rectangle>();

            float xRef = 0, yRef = 0;
            Rectangle firstRectangle = null;
            int i = 0, first = 1, m = 0, n = 0;
            nSteps = (int)(height / stepUpHeight);
            
            //Generate each rectangle starting from the lowest one
            rectangles = generateRectangles(lowerRectangleWidth, lowerRectangleLength, upperRectangleLength, upperRectangleWidth, nSteps);
            
            StringBuilder builder = new StringBuilder();

            foreach (var rectangle in rectangles)
            {
                //Determine the number of circles that can fit in the rectangle lengthwise and widthwise
                nCirclesWidth = (int)((rectangle.getWidth() / (2.00 * radius)));
                nCirclesLength = (int)((rectangle.getLength() / (2.00 * radius)));
                
                //Determine how much space is left over
                remainderWidth = rectangle.getWidth() % (nCirclesWidth * (2 * radius));
                remainderLength = rectangle.getLength() % (nCirclesLength * (2 * radius));
               
                if (first == 1)
                    firstRectangle = rectangle;

                first = 0;

                //Offset each reactangle by position of lowest rectangel
                yRef = (firstRectangle.getWidth() - rectangle.getWidth()) / 2;
                xRef = (firstRectangle.getLength() - rectangle.getLength()) / 2;

                if (pathDirection == "Horizontal")
                    for (int j = 0; j < nCirclesWidth; ++j)
                    {
                        Coordinate point2 = new DepositionMaterial.Coordinate();

                        for (int k = 0; k < nCirclesLength; ++k)
                        {
                            //Set x coordinate of material deposition. Alternate left/right when end is reached
                            if (m == 0)
                                 point2.setX((remainderLength / 2 + radius + (radius * 2 * k)) + xRef);
                            
                            else
                                point2.setX((remainderLength / 2 + radius + (radius * 2 * (nCirclesLength - k - 1))) + xRef);

                            //Set Y coordinate of material deposition. Alternated up/down when end is reached
                            if (n == 0)              
                                point2.setY(remainderWidth / 2 + radius + 2 * radius * (j) + yRef);

                            else                        
                                point2.setY(remainderWidth / 2 + radius + 2 * radius * (nCirclesWidth - j - 1) + yRef);
                                                        
                            //Z coordinate is based on height of rectangle
                            point2.setZ(stepUpHeight * i);
                            //Add coordinate to list
                            coordinates.Add(point2);
                            builder.Append(point2.printCoordinate());
                            builder.AppendLine();
                        }

                        if (m == 0)
                            m = 1;
                        else
                            m = 0;
                    }

                    //If pathdirection is vertical, reverse the order of the loops
                    if (pathDirection == "Vertical")
                            for (int j = 0; j < nCirclesLength; ++j)
                            {
                                Coordinate point3 = new DepositionMaterial.Coordinate();

                                for (int k = 0; k < nCirclesWidth; ++k)
                                {
                                    if (m == 0)                          
                                        point3.setY((remainderWidth / 2 + radius + (radius * 2 * k)) + yRef);                                    
                                    else                   
                                        point3.setY((remainderWidth / 2 + radius + (radius * 2 * (nCirclesWidth - k - 1))) + yRef);
                                   
                                    if (n == 0)   
                                         point3.setX(remainderLength / 2 + radius + 2 * radius * (j) + xRef);                                 

                                    else                               
                                        point3.setX(remainderLength / 2 + radius + 2 * radius * (nCirclesLength - j - 1) + xRef);
                                    
                                    point3.setZ(stepUpHeight * i);

                                    coordinates.Add(point3);
                                    builder.Append(point3.printCoordinate());
                                    builder.AppendLine();

                                }

                    if (m == 0)
                        m = 1;
                    else
                        m = 0;
                }

                if (n == 0)
                    n = 1;
                else
                    n = 0;

                //Next rectangle is reached. Increment i to increase the step up height
                ++i;
            }

            //Write all the coordinates to file            
            System.IO.Directory.CreateDirectory(@"C:\Coordinates");
            System.IO.File.WriteAllText(@"C:\Coordinates\path.txt", builder.ToString());
            MessageBox.Show("Coordinates successfully created at C:\\Coordinates\\path.txt");

        }
        static List<Rectangle> generateRectangles(float lowerRectangleWidth, float lowerRectangleLength, float upperRectangleLength, float upperRectangleWidth, int nSteps)
        {
            //List that holds all the rectanges
            List<Rectangle> lRect = new List<DepositionMaterial.Rectangle>();
            float increaseX, increaseY;

            //Determine size change in rectangle for width and length as each step is taken
            increaseX = (upperRectangleLength - lowerRectangleLength) / (nSteps - 1);
            increaseY = (upperRectangleWidth - lowerRectangleWidth) / (nSteps - 1);
            for (int i = 0; i < nSteps; ++i)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.setLength(lowerRectangleLength + increaseX * i);
                rectangle.setWidth(lowerRectangleWidth + increaseY * i);
                lRect.Add(rectangle);
            }

            return lRect;
        }

    }
}
