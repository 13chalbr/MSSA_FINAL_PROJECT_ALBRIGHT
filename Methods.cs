using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSA_FINAL_PROJECT_WORKING
{
    internal class Methods
    {
        public static List<double> GravAccelComp(in int n, in List<double> PosiVeli, in List<double> MassList)
        {
            List<double> VeliAcceli = new List<double>();
            int n_bodies = n;
            int iOffset, jOffset;
            double dx, dy, dz, r, ax, ay, az;
            double gravConst = Constants.GRAV;

            // Initialize VeliAcceli with zeros
            for (int i = 0; i < PosiVeli.Count; i++)
            {
                VeliAcceli.Add(0);
            }

            // Debug: Check initial list lengths
            if (PosiVeli.Count != 6 * n)
            {
                throw new ArgumentException($"PosiVeli list length is incorrect. Expected {6 * n}, but got {PosiVeli.Count}.");
            }
            if (MassList.Count != n)
            {
                throw new ArgumentException($"MassList length is incorrect. Expected {n}, but got {MassList.Count}.");
            }

            for (int i = 0; i < n_bodies; i++)
            {
                iOffset = i * 6;
                for (int j = 0; j < n_bodies; j++)
                {
                    jOffset = j * 6;

                    // Debug: Check index bounds
                    if (iOffset + 5 >= PosiVeli.Count || jOffset + 5 >= PosiVeli.Count)
                    {
                        throw new IndexOutOfRangeException($"Index out of range in PosiVeli list. iOffset: {iOffset}, jOffset: {jOffset}, PosiVeli.Count: {PosiVeli.Count}");
                    }
                    if (j >= MassList.Count)
                    {
                        throw new IndexOutOfRangeException($"Index out of range in MassList. j: {j}, MassList.Count: {MassList.Count}");
                    }

                    VeliAcceli[iOffset] = PosiVeli[iOffset + 3]; // moves v_x to first position output
                    VeliAcceli[iOffset + 1] = PosiVeli[iOffset + 4];
                    VeliAcceli[iOffset + 2] = PosiVeli[iOffset + 5];

                    if (i != j)
                    {
                        dx = PosiVeli[iOffset] - PosiVeli[jOffset];        // x dist between two bodies
                        dy = PosiVeli[iOffset + 1] - PosiVeli[jOffset + 1]; // y dist between two bodies
                        dz = PosiVeli[iOffset + 2] - PosiVeli[jOffset + 2];  // z dist between two bodies
                        r = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));     // calcs radial distance
                        ax = ((-gravConst * MassList[j]) / (r * r * r)) * dx;
                        ay = ((-gravConst * MassList[j]) / (r * r * r)) * dy;  // M*G/(r^2) * (dx..y..z/r) to get axial acceleration
                        az = ((-gravConst * MassList[j]) / (r * r * r)) * dz;
                        VeliAcceli[iOffset + 3] += ax;
                        VeliAcceli[iOffset + 4] += ay;
                        VeliAcceli[iOffset + 5] += az;
                    }   //return vec format: v_x, v_y, v_z, a_x, a_y, a_z
                }
            }
            return VeliAcceli;
        }

        public static List<double> RK4Compute(in List<double> PosiVeli, in List<double> MassList, in double dt_step, in int n)
        {
            double dt = dt_step;
            // k1:
            List<double> k1 = Methods.GravAccelComp(n, PosiVeli, MassList);  //k1 = dt x f(t_n, y_n)
            k1 = k1.Select(x => x * dt).ToList();
            // k2:
            List<double> k1half = k1.Select(x => x * 0.5).ToList();
            List<double> y_Plus_k1half = PosiVeli.Zip(k1half, (x, y) => x + y).ToList();
            List<double> k2 = Methods.GravAccelComp(n, y_Plus_k1half, MassList);
            k2 = k2.Select(x => x * (1.0 * dt)).ToList();        //k2 = dt x f(t_n+dt/2, y_n+k1/2)
            // k3:
            List<double> k2half = k2.Select(x => x * 0.5).ToList();
            List<double> y_Plus_k2half = PosiVeli.Zip(k2half, (x, y) => x + y).ToList();
            List<double> k3 = Methods.GravAccelComp(n, y_Plus_k2half, MassList);
            k3 = k3.Select(x => x * (1.0 * dt)).ToList();        //k3 = dt x f(t_n+dt/2, y_n+k2/2)
            // k4:
            List<double> y_Plus_k3 = PosiVeli.Zip(k3, (x, y) => x + y).ToList();
            List<double> k4 = Methods.GravAccelComp(n, y_Plus_k3, MassList);
            k4 = k4.Select(x => x * dt).ToList();       //k4 = dt x f(t_n+dt, y_n+k3)
            // k1+2k2+2k3+k4:
            List<double> k2doubled = k2.Select(x => x * 2).ToList();
            List<double> k3doubled = k3.Select(x => x * 2).ToList();
            List<double> k1_2k2_2k3_k4 = new List<double>();
            for (int i = 0; i < k1.Count; i++)
            {
                double sum = k1[i] + k2doubled[i] + k3doubled[i] + k4[i];
                k1_2k2_2k3_k4.Add(sum);
            }
            // return y_(n+1):
            List<double> onesixth_k1_2k2_2k3_k4 = k1_2k2_2k3_k4.Select(x => x * (0.166666666666667)).ToList();  // y_(n+1) = y_n + (1/6)(k1+2k2+2k3+k4)
            List<double> RK4d_PosiVel = PosiVeli.Zip(onesixth_k1_2k2_2k3_k4, (x, y) => x + y).ToList();
            return RK4d_PosiVel;
        }

        public static List<List<List<double>>> TransposePast(in List<double> SystemPosVel_PAST, in int n)
        {
            //Intialize a list of lists to hold our x, y, and z coords:
            List<List<List<double>>> splitListOFLists = new List<List<List<double>>>(); //Initializes a "list of lists of lists..." to hold our calc values.
            for (int i = 0; i < n; i++)
            {
                splitListOFLists.Add(new List<List<double>>());
                for (int j = 0; j < 3; j++)
                {
                    splitListOFLists[i].Add(new List<double>());
                }
            }
            int planetIndex = 0; // aritifial pointer tracks which plant list to funnel entries to. 

            for (int i = 0; i < SystemPosVel_PAST.Count; i += 6)
            {
                for (int j = i; j < i + 3 && j < SystemPosVel_PAST.Count; j++)  // cuts out all the velocity components
                {
                    splitListOFLists[planetIndex][j - i].Add((SystemPosVel_PAST[j]) / (Constants.AU));  // casts into AU for plotting

                    if (j == i + 2)
                    {
                        planetIndex++;
                    }
                    if (planetIndex == n)
                    {
                        planetIndex = 0;
                    }
                }
            }
            return splitListOFLists;
        }
        public static void TransposeMixer(in List<List<List<double>>> split_PAST, in int n, out List<List<double>> xList, out List<List<double>> yList, out List<List<double>> zList)
        {
            xList = new List<List<double>>();
            for (int i = 0; i < n; i++)
            {
                xList.Add(new List<double>());
            }
            yList = new List<List<double>>();
            for (int i = 0; i < n; i++)
            {
                yList.Add(new List<double>());
            }
            zList = new List<List<double>>();
            for (int i = 0; i < n; i++)
            {
                zList.Add(new List<double>());
            }
            for (int i = 0; i < n; ++i)
            {
                xList.Add(split_PAST[i][0]);
                yList.Add(split_PAST[i][1]);
                zList.Add(split_PAST[i][2]);
            }
        }
    }
}
