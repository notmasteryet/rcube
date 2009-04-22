using System;
using System.Collections.Generic;
using System.Text;

namespace RCube
{
    class Matrix : ICloneable
    {
        private double[,] _m;
        private int n;
        private int m;

        public int N
        {
            get { return n; }
        }

        public int M
        {
            get { return m; }
        }

        public double this[int i, int j] 
        {
            get { return _m[i, j]; }
            set { _m[i, j] = value; }
        }

        public Matrix(int n, int m)
        {
            this.n = n;
            this.m = m;
            _m = new double[n, m];
        }

        public Matrix(double[,] a, bool own)
        {
            this.n = a.GetLength(0);
            this.m = a.GetLength(1);
            _m = own ? a : (double[,])a.Clone();
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.M != B.N) throw new InvalidOperationException("Noncombatible dimentions");
            double[,] c = new double[A.N, B.M];
            for (int i = 0; i < A.N; i++)
            {
                for (int j = 0; j < B.M; j++)
                {
                    double sum = 0;
                    for (int q = 0; q < A.M; q++)
                    {
                        sum += A[i, q] * B[q, j];
                    }
                    c[i, j] = sum;
                }
            }
            return new Matrix(c, true);
        }

        public static Matrix operator *(double k, Matrix A)
        {
            double[,] c = new double[A.N, A.M];
            for (int i = 0; i < A.N; i++)
            {
                for (int j = 0; j < A.M; j++)
                {
                    c[i, j] = k * A[i,j];
                }
            }
            return new Matrix(c, true);
        }

        public static Matrix operator /(double k, Matrix A)
        {
            if (A.N != A.M) throw new InvalidOperationException("Valid only for square matrix");

            int d = A.N;
            double[,] m = (double[,])A._m.Clone();
            double[,] mOposite = new double[d, d];
            for (int i = 0; i < d; i++)
            {
                mOposite[i, i] = k;
            }

            for (int i = 0; i < d; i++)
            {
                if (m[i, i] == 0)
                {
                    // set non-zero on diagonal
                    for (int i0 = i + 1; i0 < d; i0++)
                    {
                        if (m[i, i0] != 0)
                        {
                            for (int j0 = 0; j0 < d; j0++)
                            {
                                double t;
                                t = m[i, j0]; m[i, j0] = m[i0, j0]; m[i0, j0] = t;
                                t = mOposite[i, j0]; mOposite[i, j0] = mOposite[i0, j0]; mOposite[i0, j0] = t;
                            }
                            break;
                        }
                    }
                }

                double r = m[i, i];
                m[i, i] = 1.0;
                for (int j0 = i + 1; j0 < d; j0++)
                {
                    m[i, j0] /= r;
                }
                for (int j0 = 0; j0 < d; j0++)
                {
                    mOposite[i, j0] /= r;
                }
                for (int i0 = 0; i0 < d; i0++)
                {
                    if (i != i0 & m[i0, i] != 0)
                    {
                        double r0 = m[i0, i];
                        for (int j0 = i; j0 < d; j0++)
                        {
                            m[i0, j0] -= m[i, j0] * r0;
                        }
                        for (int j0 = 0; j0 < d; j0++)
                        {
                            mOposite[i0, j0] -= mOposite[i, j0] * r0;
                        }
                    }
                }
            }

            return new Matrix(mOposite, true);  
        }

        public double Abs()
        {
            if(N != M) throw new InvalidOperationException("Valid only for square matrix");
            return CalcAbs(0, new System.Collections.BitArray(N));
        }

        private double CalcAbs(int i, System.Collections.BitArray used)
        {
            if (i == N - 1)
            {
                int j = 0;
                while (used[j]) j++;
                return _m[i, j];
            }
            else
            {
                int a = 1;
                double sum = 0;
                for (int j = 0; j < N; j++)
                {
                    if (!used[j])
                    {
                        if (_m[i, j] != 0)
                        {
                            used.Set(j, true);
                            sum += a * _m[i, j] * CalcAbs(i + 1, used);
                            used.Set(j, false);
                        }
                        a = -a;
                    }
                }
                return sum;
            }
        }


        #region ICloneable Members

        public object Clone()
        {
            return new Matrix(_m, false);
        }

        #endregion
    }
}
