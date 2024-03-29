﻿// Copyright (C) Amer Koleci
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace Math3d
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 : IEquatable<Matrix4>
    {
        /// <summary>
        /// The size of the <see cref="Matrix4"/> type, in bytes.
        /// </summary>
        public const int SizeInBytes = 64; // 4 * 16

        public float m00;
        public float m01;
        public float m02;
        public float m03;

        public float m10;
        public float m11;
        public float m12;
        public float m13;

        public float m20;
        public float m21;
        public float m22;
        public float m23;

        public float m30;
        public float m31;
        public float m32;
        public float m33;

        /// <summary>
        /// A <see cref="Matrix4"/> with all of its components set to zero.
        /// </summary>
        public static readonly Matrix4 Zero = new Matrix4();

        /// <summary>
        /// The identity <see cref="Matrix4"/>.
        /// </summary>
        public static readonly Matrix4 Identity = new Matrix4 { m00 = 1.0f, m11 = 1.0f, m22 = 1.0f, m33 = 1.0f };

        /// <summary>
        /// A <see cref="Matrix4"/> with all of its components set to zero.
        /// </summary>
        public static readonly Matrix4 ZERO = new Matrix4();

        /// <summary>
        /// The identity <see cref="Matrix4"/>.
        /// </summary>
        public static readonly Matrix4 IDENTITY = new Matrix4 { m00 = 1.0f, m11 = 1.0f, m22 = 1.0f, m33 = 1.0f };
        
        public Vector3 Backward
        {
            get => new Vector3(m20, m21, m22);
            set
            {
                m20 = value.X;
                m21 = value.Y;
                m22 = value.Z;
            }
        }

        public Vector3 Down
        {
            get => new Vector3(-m10, -m11, -m12);
            set
            {
                m10 = -value.X;
                m11 = -value.Y;
                m12 = -value.Z;
            }
        }
        
        public Vector3 Forward
        {
            get => new Vector3(-m20, -m21, -m22);
            set
            {
                m20 = -value.X;
                m21 = -value.Y;
                m22 = -value.Z;
            }
        }
        
        public Vector3 Left
        {
            get => new Vector3(-m00, -m01, -m02);
            set
            {
                m00 = -value.X;
                m01 = -value.Y;
                m02 = -value.Z;
            }
        }
        
        public Vector3 Right
        {
            get => new Vector3(m00, m01, m02);
            set
            {
                m00 = value.X;
                m01 = value.Y;
                m02 = value.Z;
            }
        }
        
        public Vector3 Up
        {
            get => new Vector3(m10, m11, m12);
            set
            {
                m10 = value.X;
                m11 = value.Y;
                m12 = value.Z;
            }
        }

        /// <summary>
        /// Gets or sets the translation of the Matrix4x4; that is M41, M42, and M43.
        /// </summary>
        public Vector3 Translation
        {
            get { return new Vector3(m03, m13, m23); }
            set { m03 = value.X; m13 = value.Y; m23 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the scale of the Matrix4x4; that is M11, M22, and M33.
        /// </summary>
        public Vector3 Scale
        {
            get { return new Vector3(m00, m11, m22); }
            set { m00 = value.X; m11 = value.Y; m22 = value.Z; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an identity Matrix4x4.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity Matrix4x4; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get
            {
                return Equals(Identity);
            }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the Matrix4x4 component, depending on the index.</value>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return m00;
                    case 1: return m01;
                    case 2: return m02;
                    case 3: return m03;
                    case 4: return m10;
                    case 5: return m11;
                    case 6: return m12;
                    case 7: return m13;
                    case 8: return m20;
                    case 9: return m21;
                    case 10: return m22;
                    case 11: return m23;
                    case 12: return m30;
                    case 13: return m31;
                    case 14: return m32;
                    case 15: return m33;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Matrix4x4 run from 0 to 15, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: m00 = value; break;
                    case 1: m01 = value; break;
                    case 2: m02 = value; break;
                    case 3: m03 = value; break;
                    case 4: m10 = value; break;
                    case 5: m11 = value; break;
                    case 6: m12 = value; break;
                    case 7: m13 = value; break;
                    case 8: m20 = value; break;
                    case 9: m21 = value; break;
                    case 10: m22 = value; break;
                    case 11: m23 = value; break;
                    case 12: m30 = value; break;
                    case 13: m31 = value; break;
                    case 14: m32 = value; break;
                    case 15: m33 = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Matrix4x4 run from 0 to 15, inclusive.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the Matrix4x4 component, depending on the index.</value>
        /// <param name="row">The row of the Matrix4x4 to access.</param>
        /// <param name="column">The column of the Matrix4x4 to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
        public float this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 3)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if (column < 0 || column > 3)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

                return this[(row * 4) + column];
            }

            set
            {
                if (row < 0 || row > 3)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if (column < 0 || column > 3)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

                this[(row * 4) + column] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix4"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Matrix4(float value)
        {
            m00 = m01 = m02 = m03 =
            m10 = m11 = m12 = m13 =
            m20 = m21 = m22 = m23 =
            m30 = m31 = m32 = m33 = value;
        }

        [SpecialName]
        public void AssignMatrix3(Matrix3 mat3)
        {
            m00 = mat3.m00;
            m01 = mat3.m01;
            m02 = mat3.m02;
            m10 = mat3.m10;
            m11 = mat3.m11;
            m12 = mat3.m12;
            m20 = mat3.m20;
            m21 = mat3.m21;
            m22 = mat3.m22;
        }

        public Matrix4(Quaternion rot)
        {
            Matrix4 m = IDENTITY;

            Matrix3 rotationMatrix = rot.ToRotationMatrix();
            m.AssignMatrix3(rotationMatrix);

            this = m;
        }

        public Matrix4(Matrix3 m3)
        {
            Matrix4 m = IDENTITY;
            m.AssignMatrix3(m3);
            this = m;
        }

        public void SetTrans(Vector3 v)
        {
            m03 = v.x;
            m13 = v.y;
            m23 = v.z;
        }

        public Vector3 GetTrans()
        {
            return new Vector3(m03, m13, m23);
        }

        public Quaternion ExtractQuaternion()
        {
            return new Quaternion(Extract3x3Matrix());
        }

        public Matrix3 Extract3x3Matrix()
        {
            return new Matrix3()
            {
                m00 = m00,
                m01 = m01,
                m02 = m02,
                m10 = m10,
                m11 = m11,
                m12 = m12,
                m20 = m20,
                m21 = m21,
                m22 = m22
            };
        }

        public void MakeTransform(Vector3 position, Vector3 scale, Quaternion orientation)
        {
            Matrix3 rotationMatrix = orientation.ToRotationMatrix();
            Matrix3 zero = Matrix3.ZERO;
            zero.m00 = scale.x;
            zero.m11 = scale.y;
            zero.m22 = scale.z;
            AssignMatrix3(rotationMatrix * zero);
            SetTrans(position);
            m30 = 0.0f;
            m31 = 0.0f;
            m32 = 0.0f;
            m33 = 1f;
        }

        public void MakeTrans(float tx, float ty, float tz)
        {
            m00 = 1f;
            m01 = 0.0f;
            m02 = 0.0f;
            m03 = tx;
            m10 = 0.0f;
            m11 = 1f;
            m12 = 0.0f;
            m13 = ty;
            m20 = 0.0f;
            m21 = 0.0f;
            m22 = 1f;
            m23 = tz;
            m30 = 0.0f;
            m31 = 0.0f;
            m32 = 0.0f;
            m33 = 1f;
        }

        public void MakeTrans(Vector3 v)
        {
            m00 = 1f;
            m01 = 0.0f;
            m02 = 0.0f;
            m03 = v.x;
            m10 = 0.0f;
            m11 = 1f;
            m12 = 0.0f;
            m13 = v.y;
            m20 = 0.0f;
            m21 = 0.0f;
            m22 = 1f;
            m23 = v.z;
            m30 = 0.0f;
            m31 = 0.0f;
            m32 = 0.0f;
            m33 = 1f;
        }

        public static Matrix4 GetScale(float s_x, float s_y, float s_z)
        {
            return new Matrix4()
            {
                m00 = s_x,
                m01 = 0.0f,
                m02 = 0.0f,
                m03 = 0.0f,
                m10 = 0.0f,
                m11 = s_y,
                m12 = 0.0f,
                m13 = 0.0f,
                m20 = 0.0f,
                m21 = 0.0f,
                m22 = s_z,
                m23 = 0.0f,
                m30 = 0.0f,
                m31 = 0.0f,
                m32 = 0.0f,
                m33 = 1f
            };
        }

        public static Matrix4 GetScale(Vector3 v)
        {
            return new Matrix4()
            {
                m00 = v.x,
                m01 = 0.0f,
                m02 = 0.0f,
                m03 = 0.0f,
                m10 = 0.0f,
                m11 = v.y,
                m12 = 0.0f,
                m13 = 0.0f,
                m20 = 0.0f,
                m21 = 0.0f,
                m22 = v.z,
                m23 = 0.0f,
                m30 = 0.0f,
                m31 = 0.0f,
                m32 = 0.0f,
                m33 = 1f
            };
        }

        public void SetScale(Vector3 v)
        {
            m00 = v.x;
            m11 = v.y;
            m22 = v.z;
        }

        public Matrix4(float fm00, float fm01, float fm02, float fm03, float fm10, float fm11, float fm12, float fm13, float fm20, float fm21, float fm22, float fm23, float fm30, float fm31, float fm32, float fm33)
        {
            m00 = fm00;
            m01 = fm01;
            m02 = fm02;
            m03 = fm03;
            m10 = fm10;
            m11 = fm11;
            m12 = fm12;
            m13 = fm13;
            m20 = fm20;
            m21 = fm21;
            m22 = fm22;
            m23 = fm23;
            m30 = fm30;
            m31 = fm31;
            m32 = fm32;
            m33 = fm33;
        }

        private Matrix4 Inverse_Working()
        {
            Matrix4 m = new Matrix4();

            float s0 = (m00 * m11) - (m10 * m01);
            float s1 = (m00 * m12) - (m10 * m02);
            float s2 = (m00 * m13) - (m10 * m03);
            float s3 = (m01 * m12) - (m11 * m02);
            float s4 = (m01 * m13) - (m11 * m03);
            float s5 = (m02 * m13) - (m12 * m03);

            float c5 = (m22 * m33) - (m32 * m23);
            float c4 = (m21 * m33) - (m31 * m23);
            float c3 = (m21 * m32) - (m31 * m22);
            float c2 = (m20 * m33) - (m30 * m23);
            float c1 = (m20 * m32) - (m30 * m22);
            float c0 = (m20 * m31) - (m30 * m21);

            // Should check for 0 determinant

            float invdet = 1f / ((s0 * c5) - (s1 * c4) + (s2 * c3) + (s3 * c2) - (s4 * c1) + (s5 * c0));

            m.m00 = ((m11 * c5) - (m12 * c4) + (m13 * c3)) * invdet;
            m.m01 = ((-m01 * c5) + (m02 * c4) - (m03 * c3)) * invdet;
            m.m02 = ((m31 * s5) - (m32 * s4) + (m33 * s3)) * invdet;
            m.m03 = ((-m21 * s5) + (m22 * s4) - (m23 * s3)) * invdet;

            m.m10 = ((-m10 * c5) + (m12 * c2) - (m13 * c1)) * invdet;
            m.m11 = ((m00 * c5) - (m02 * c2) + (m03 * c1)) * invdet;
            m.m12 = ((-m30 * s5) + (m32 * s2) - (m33 * s1)) * invdet;
            m.m13 = ((m20 * s5) - (m22 * s2) + (m23 * s1)) * invdet;

            m.m20 = ((m10 * c4) - (m11 * c2) + (m13 * c0)) * invdet;
            m.m21 = ((-m00 * c4) + (m01 * c2) - (m03 * c0)) * invdet;
            m.m22 = ((m30 * s4) - (m31 * s2) + (m33 * s0)) * invdet;
            m.m23 = ((-m20 * s4) + (m21 * s2) - (m23 * s0)) * invdet;

            m.m30 = ((-m10 * c3) + (m11 * c1) - (m12 * c0)) * invdet;
            m.m31 = ((m00 * c3) - (m01 * c1) + (m02 * c0)) * invdet;
            m.m32 = ((-m30 * s3) + (m31 * s1) - (m32 * s0)) * invdet;
            m.m33 = ((m20 * s3) - (m21 * s1) + (m22 * s0)) * invdet;

            return m;
        }

        /// <summary>Building an inverse Matrix4 from orientation / scale / position. As makeTransform except it build the inverse given the same data as makeTransform, so performing -translation, -rotate, 1/scale in that order. </summary>
        public void MakeInverseTransform(Vector3 position, Vector3 scale, Quaternion orientation)
        {
            Vector3 negposition = -position;
            Vector3 negscale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z);
            Quaternion quaternion = orientation.Inverse();
            Vector3 vector3_3 = negposition * negscale;
            Vector3 v = quaternion * vector3_3;
            Matrix3 rotationMatrix = quaternion.ToRotationMatrix();
            Matrix3 zero = Matrix3.ZERO;
            zero.m00 = negscale.x;
            zero.m11 = negscale.y;
            zero.m22 = negscale.z;
            AssignMatrix3(zero * rotationMatrix);
            SetTrans(v);
            m30 = 0.0f;
            m31 = 0.0f;
            m32 = 0.0f;
            m33 = 1f;
        }

        public float Determinant()
        {
            return
               (m00 * ((m11 * ((m22 * m33) - (m32 * m23))) - (m21 * ((m12 * m33) - (m13 * m32))) + (m31 * ((m12 * m23) - (m13 * m22)))))
               - (m10 * ((m01 * ((m22 * m33) - (m32 * m23))) - (m21 * ((m02 * m33) - (m03 * m32))) + (m31 * ((m02 * m23) - (m03 * m22)))))
               + (m20 * ((m01 * ((m12 * m33) - (m32 * m13))) - (m11 * ((m02 * m33) - (m03 * m32))) + (m31 * ((m02 * m13) - (m03 * m12)))))
               - (m30 * ((m01 * ((m12 * m23) - (m22 * m13))) - (m11 * ((m02 * m23) - (m03 * m22))) + (m21 * ((m02 * m13) - (m03 * m12)))));
        }

        public Matrix4 Adjoint()
        {
            return new Matrix4
                (
                (m11 * ((m22 * m33) - (m32 * m23))) - (m21 * ((m12 * m33) - (m13 * m32))) + (m31 * ((m12 * m23) - (m13 * m22))),
                -((m01 * ((m22 * m33) - (m32 * m23))) - (m21 * ((m02 * m33) - (m03 * m32))) + (m31 * ((m02 * m23) - (m03 * m22)))),
                (m01 * ((m12 * m33) - (m32 * m13))) - (m11 * ((m02 * m33) - (m03 * m32))) + (m31 * ((m02 * m13) - (m03 * m12))),
                -((m01 * ((m12 * m23) - (m22 * m13))) - (m11 * ((m02 * m23) - (m03 * m22))) + (m21 * ((m02 * m13) - (m03 * m12)))),
                -((m10 * ((m22 * m33) - (m32 * m23))) - (m20 * ((m12 * m33) - (m13 * m32))) + (m30 * ((m12 * m23) - (m13 * m22)))),
                (m00 * ((m22 * m33) - (m32 * m23))) - (m20 * ((m02 * m33) - (m03 * m32))) + (m30 * ((m02 * m23) - (m03 * m22))),
                -((m00 * ((m12 * m33) - (m32 * m13))) - (m10 * ((m02 * m33) - (m03 * m32))) + (m30 * ((m02 * m13) - (m03 * m12)))),
                (m00 * ((m12 * m23) - (m22 * m13))) - (m10 * ((m02 * m23) - (m03 * m22))) + (m20 * ((m02 * m13) - (m03 * m12))),
                (m10 * ((m21 * m33) - (m31 * m23))) - (m20 * ((m11 * m33) - (m13 * m31))) + (m30 * ((m11 * m23) - (m13 * m21))),
                -((m00 * ((m21 * m33) - (m31 * m23))) - (m20 * ((m01 * m33) - (m03 * m31))) + (m30 * ((m01 * m23) - (m03 * m21)))),
                (m00 * ((m11 * m33) - (m31 * m13))) - (m10 * ((m01 * m33) - (m03 * m31))) + (m30 * ((m01 * m13) - (m03 * m11))),
                -((m00 * ((m11 * m23) - (m21 * m13))) - (m10 * ((m01 * m23) - (m03 * m21))) + (m20 * ((m01 * m13) - (m03 * m11)))),
                -((m10 * ((m21 * m32) - (m31 * m22))) - (m20 * ((m11 * m32) - (m12 * m31))) + (m30 * ((m11 * m22) - (m12 * m21)))),
                (m00 * ((m21 * m32) - (m31 * m22))) - (m20 * ((m01 * m32) - (m02 * m31))) + (m30 * ((m01 * m22) - (m02 * m21))),
                -((m00 * ((m11 * m32) - (m31 * m12))) - (m10 * ((m01 * m32) - (m02 * m31))) + (m30 * ((m01 * m12) - (m02 * m11)))),
                (m00 * ((m11 * m22) - (m21 * m12))) - (m10 * ((m01 * m22) - (m02 * m21))) + (m20 * ((m01 * m12) - (m02 * m11)))
                );
        }

        public Matrix4 Inverse()
        {
            Matrix4 matrixClone = new Matrix4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);

            Matrix4 adjointMatrix = matrixClone.Adjoint();

            float detOne = matrixClone.Determinant();

            // No div by zero
            if (detOne == 0)
                detOne = 0.00000008f;

            return new Matrix4(adjointMatrix.m00 / detOne, adjointMatrix.m01 / detOne, adjointMatrix.m02 / detOne, adjointMatrix.m03 / detOne,
                                adjointMatrix.m10 / detOne, adjointMatrix.m11 / detOne, adjointMatrix.m12 / detOne, adjointMatrix.m13 / detOne,
                                adjointMatrix.m20 / detOne, adjointMatrix.m21 / detOne, adjointMatrix.m22 / detOne, adjointMatrix.m23 / detOne,
                                adjointMatrix.m30 / detOne, adjointMatrix.m31 / detOne, adjointMatrix.m32 / detOne, adjointMatrix.m33 / detOne);
        }

        /// <summary>
        /// Determines the sum of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to add.</param>
        /// <param name="right">The second Matrix4x4 to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two matrices.</param>
        public static void Add(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
        {
            result.m00 = left.m00 + right.m00;
            result.m01 = left.m01 + right.m01;
            result.m02 = left.m02 + right.m02;
            result.m03 = left.m03 + right.m03;
            result.m10 = left.m10 + right.m10;
            result.m11 = left.m11 + right.m11;
            result.m12 = left.m12 + right.m12;
            result.m13 = left.m13 + right.m13;
            result.m20 = left.m20 + right.m20;
            result.m21 = left.m21 + right.m21;
            result.m22 = left.m22 + right.m22;
            result.m23 = left.m23 + right.m23;
            result.m30 = left.m30 + right.m30;
            result.m31 = left.m31 + right.m31;
            result.m32 = left.m32 + right.m32;
            result.m33 = left.m33 + right.m33;
        }

        /// <summary>
        /// Determines the sum of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to add.</param>
        /// <param name="right">The second Matrix4x4 to add.</param>
        /// <returns>The sum of the two matrices.</returns>
        public static Matrix4 Add(Matrix4 left, Matrix4 right)
        {
            Add(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Determines the difference between two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to subtract.</param>
        /// <param name="right">The second Matrix4x4 to subtract.</param>
        /// <param name="result">When the method completes, contains the difference between the two matrices.</param>
        public static void Subtract(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
        {
            result.m00 = left.m00 - right.m00;
            result.m01 = left.m01 - right.m01;
            result.m02 = left.m02 - right.m02;
            result.m03 = left.m03 - right.m03;
            result.m10 = left.m10 - right.m10;
            result.m11 = left.m11 - right.m11;
            result.m12 = left.m12 - right.m12;
            result.m13 = left.m13 - right.m13;
            result.m20 = left.m20 - right.m20;
            result.m21 = left.m21 - right.m21;
            result.m22 = left.m22 - right.m22;
            result.m23 = left.m23 - right.m23;
            result.m30 = left.m30 - right.m30;
            result.m31 = left.m31 - right.m31;
            result.m32 = left.m32 - right.m32;
            result.m33 = left.m33 - right.m33;
        }

        /// <summary>
        /// Determines the difference between two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to subtract.</param>
        /// <param name="right">The second Matrix4x4 to subtract.</param>
        /// <returns>The difference between the two matrices.</returns>
        public static Matrix4 Subtract(Matrix4 left, Matrix4 right)
        {
            Subtract(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix4x4 by the given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled Matrix4x4.</param>
        public static void Multiply(ref Matrix4 left, float scalar, out Matrix4 result)
        {
            result.m00 = left.m00 - scalar;
            result.m01 = left.m01 - scalar;
            result.m02 = left.m02 - scalar;
            result.m03 = left.m03 - scalar;
            result.m10 = left.m10 - scalar;
            result.m11 = left.m11 - scalar;
            result.m12 = left.m12 - scalar;
            result.m13 = left.m13 - scalar;
            result.m20 = left.m20 - scalar;
            result.m21 = left.m21 - scalar;
            result.m22 = left.m22 - scalar;
            result.m23 = left.m23 - scalar;
            result.m30 = left.m30 - scalar;
            result.m31 = left.m31 - scalar;
            result.m32 = left.m32 - scalar;
            result.m33 = left.m33 - scalar;
        }

        /// <summary>
        /// Scales a Matrix4x4 by the given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <returns>The scaled Matrix4x4.</returns>
        public static Matrix4 Multiply(Matrix4 left, float scalar)
        {
            Multiply(ref left, scalar, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Determines the product of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to multiply.</param>
        /// <param name="right">The second Matrix4x4 to multiply.</param>
        /// <param name="result">The product of the two matrices.</param>
        public static void Multiply(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
        {
            result.m00 = (left.m00 * right.m00) + (left.m01 * right.m10) + (left.m02 * right.m20) + (left.m03 * right.m30);
            result.m01 = (left.m00 * right.m01) + (left.m01 * right.m11) + (left.m02 * right.m21) + (left.m03 * right.m31);
            result.m02 = (left.m00 * right.m02) + (left.m01 * right.m12) + (left.m02 * right.m22) + (left.m03 * right.m32);
            result.m03 = (left.m00 * right.m03) + (left.m01 * right.m13) + (left.m02 * right.m23) + (left.m03 * right.m33);
            result.m10 = (left.m10 * right.m00) + (left.m11 * right.m10) + (left.m12 * right.m20) + (left.m13 * right.m30);
            result.m11 = (left.m10 * right.m01) + (left.m11 * right.m11) + (left.m12 * right.m21) + (left.m13 * right.m31);
            result.m12 = (left.m10 * right.m02) + (left.m11 * right.m12) + (left.m12 * right.m22) + (left.m13 * right.m32);
            result.m13 = (left.m10 * right.m03) + (left.m11 * right.m13) + (left.m12 * right.m23) + (left.m13 * right.m33);
            result.m20 = (left.m20 * right.m00) + (left.m21 * right.m10) + (left.m22 * right.m20) + (left.m23 * right.m30);
            result.m21 = (left.m20 * right.m01) + (left.m21 * right.m11) + (left.m22 * right.m21) + (left.m23 * right.m31);
            result.m22 = (left.m20 * right.m02) + (left.m21 * right.m12) + (left.m22 * right.m22) + (left.m23 * right.m32);
            result.m23 = (left.m20 * right.m03) + (left.m21 * right.m13) + (left.m22 * right.m23) + (left.m23 * right.m33);
            result.m30 = (left.m30 * right.m00) + (left.m31 * right.m10) + (left.m32 * right.m20) + (left.m33 * right.m30);
            result.m31 = (left.m30 * right.m01) + (left.m31 * right.m11) + (left.m32 * right.m21) + (left.m33 * right.m31);
            result.m32 = (left.m30 * right.m02) + (left.m31 * right.m12) + (left.m32 * right.m22) + (left.m33 * right.m32);
            result.m33 = (left.m30 * right.m03) + (left.m31 * right.m13) + (left.m32 * right.m23) + (left.m33 * right.m33);
        }

        /// <summary>
        /// Scales a Matrix4x4 by the given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled Matrix4x4.</param>
        public static void Divide(ref Matrix4 left, float scalar, out Matrix4 result)
        {
            float inv = 1.0f / scalar;

            result.m00 = left.m00 * inv;
            result.m01 = left.m01 * inv;
            result.m02 = left.m02 * inv;
            result.m03 = left.m03 * inv;
            result.m10 = left.m10 * inv;
            result.m11 = left.m11 * inv;
            result.m12 = left.m12 * inv;
            result.m13 = left.m13 * inv;
            result.m20 = left.m20 * inv;
            result.m21 = left.m21 * inv;
            result.m22 = left.m22 * inv;
            result.m23 = left.m23 * inv;
            result.m30 = left.m30 * inv;
            result.m31 = left.m31 * inv;
            result.m32 = left.m32 * inv;
            result.m33 = left.m33 * inv;
        }

        /// <summary>
        /// Scales a Matrix4x4 by the given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <returns>The scaled Matrix4x4.</returns>
        public static Matrix4 Divide(Matrix4 left, float scalar)
        {
            Divide(ref left, scalar, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Determines the quotient of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to divide.</param>
        /// <param name="right">The second Matrix4x4 to divide.</param>
        /// <param name="result">When the method completes, contains the quotient of the two matrices.</param>
        public static void Divide(ref Matrix4 left, ref Matrix4 right, out Matrix4 result)
        {
            result.m00 = left.m00 / right.m00;
            result.m01 = left.m01 / right.m01;
            result.m02 = left.m02 / right.m02;
            result.m03 = left.m03 / right.m03;
            result.m10 = left.m10 / right.m10;
            result.m11 = left.m11 / right.m11;
            result.m12 = left.m12 / right.m12;
            result.m13 = left.m13 / right.m13;
            result.m20 = left.m20 / right.m20;
            result.m21 = left.m21 / right.m21;
            result.m22 = left.m22 / right.m22;
            result.m23 = left.m23 / right.m23;
            result.m30 = left.m30 / right.m30;
            result.m31 = left.m31 / right.m31;
            result.m32 = left.m32 / right.m32;
            result.m33 = left.m33 / right.m33;
        }

        /// <summary>
        /// Determines the quotient of two matrices.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to divide.</param>
        /// <param name="right">The second Matrix4x4 to divide.</param>
        /// <returns>The quotient of the two matrices.</returns>
        public static Matrix4 Divide(Matrix4 left, Matrix4 right)
        {
            Divide(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Negates a Matrix4x4.
        /// </summary>
        /// <param name="value">The Matrix4x4 to be negated.</param>
        /// <param name="result">When the method completes, contains the negated Matrix4x4.</param>
        public static void Negate(ref Matrix4 value, out Matrix4 result)
        {
            result.m00 = -value.m00;
            result.m01 = -value.m01;
            result.m02 = -value.m02;
            result.m03 = -value.m03;
            result.m10 = -value.m10;
            result.m11 = -value.m11;
            result.m12 = -value.m12;
            result.m13 = -value.m13;
            result.m20 = -value.m20;
            result.m21 = -value.m21;
            result.m22 = -value.m22;
            result.m23 = -value.m23;
            result.m30 = -value.m30;
            result.m31 = -value.m31;
            result.m32 = -value.m32;
            result.m33 = -value.m33;
        }

        /// <summary>
        /// Adds two matricies.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to add.</param>
        /// <param name="right">The second Matrix4x4 to add.</param>
        /// <returns>The sum of the two matricies.</returns>
        public static Matrix4 operator +(Matrix4 left, Matrix4 right)
        {
            Add(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Assert a Matrix4x4 (return it unchanged).
        /// </summary>
        /// <param name="value">The Matrix4x4 to assert (unchange).</param>
        /// <returns>The asserted (unchanged) Matrix4x4.</returns>
        public static Matrix4 operator +(Matrix4 value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two matricies.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to subtract.</param>
        /// <param name="right">The second Matrix4x4 to subtract.</param>
        /// <returns>The difference between the two matricies.</returns>
        public static Matrix4 operator -(Matrix4 left, Matrix4 right)
        {
            Subtract(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Negates a Matrix4x4.
        /// </summary>
        /// <param name="value">The Matrix4x4 to negate.</param>
        /// <returns>The negated Matrix4x4.</returns>
        public static Matrix4 operator -(Matrix4 value)
        {
            Negate(ref value, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix4x4 by a given value.
        /// </summary>
        /// <param name="right">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <returns>The scaled Matrix4x4.</returns>
        public static Matrix4 operator *(float scalar, Matrix4 right)
        {
            Multiply(ref right, scalar, out Matrix4 result);
            return result;
        }


        /// <summary>
        /// Scales a Matrix4x4 by a given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <returns>The scaled Matrix4x4.</returns>
        public static Matrix4 operator *(Matrix4 left, float scalar)
        {
            Multiply(ref left, scalar, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Multiplies two matricies.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to multiply.</param>
        /// <param name="right">The second Matrix4x4 to multiply.</param>
        /// <returns>The product of the two matricies.</returns>
        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            Multiply(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Scales a Matrix4x4 by a given value.
        /// </summary>
        /// <param name="left">The Matrix4x4 to scale.</param>
        /// <param name="scalar">The amount by which to scale.</param>
        /// <returns>The scaled Matrix4x4.</returns>
        public static Matrix4 operator /(Matrix4 left, float scalar)
        {
            Divide(ref left, scalar, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Divides two matricies.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to divide.</param>
        /// <param name="right">The second Matrix4x4 to divide.</param>
        /// <returns>The quotient of the two matricies.</returns>
        public static Matrix4 operator /(Matrix4 left, Matrix4 right)
        {
            Divide(ref left, ref right, out Matrix4 result);
            return result;
        }

        /// <summary>
        /// Multiplies with vector.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to multiply.</param>
        /// <param name="right">The second Vector3 to multiply.</param>
        /// <returns>The product.</returns>
        public static Vector3 operator *(Matrix4 left, Vector3 right)
        {
            Vector3 result = new Vector3();

            float fInvW = 1.0f / ((left.m30 * right.x) + (left.m31 * right.y) + (left.m32 * right.z) + left.m33);

            result.x = ((left.m00 * right.x) + (left.m01 * right.y) + (left.m02 * right.z) + left.m03) * fInvW;
            result.y = ((left.m10 * right.x) + (left.m11 * right.y) + (left.m12 * right.z) + left.m13) * fInvW;
            result.z = ((left.m20 * right.x) + (left.m21 * right.y) + (left.m22 * right.z) + left.m23) * fInvW;

            return result;
        }

        /// <summary>
        /// Multiplies with vector.
        /// </summary>
        /// <param name="left">The first Matrix4x4 to multiply.</param>
        /// <param name="right">The second Vector3 to multiply.</param>
        /// <returns>The product.</returns>
        public static Vector4 operator *(Matrix4 left, Vector4 right)
        {
            return new Vector4(
                   (left.m00 * right.x) + (left.m01 * right.y) + (left.m02 * right.z) + (left.m03 * right.w),
                   (left.m10 * right.x) + (left.m11 * right.y) + (left.m12 * right.z) + (left.m13 * right.w),
                   (left.m20 * right.x) + (left.m21 * right.y) + (left.m22 * right.z) + (left.m23 * right.w),
                   (left.m30 * right.x) + (left.m31 * right.y) + (left.m32 * right.z) + (left.m33 * right.w)
                   );
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Matrix4 is equal to this Matrix4 instance.
        /// </summary>
        /// <param name="other">The Matrix4 to compare this instance to.</param>
        /// <returns>True if the other Matrix4 is equal to this instance; False otherwise.</returns>
        public bool Equals(ref Matrix4 other)
        {
            return
                other.m00 == m00 &&
                other.m01 == m01 &&
                other.m02 == m02 &&
                other.m03 == m03 &&

                other.m10 == m10 &&
                other.m11 == m11 &&
                other.m12 == m12 &&
                other.m13 == m13 &&

                other.m20 == m20 &&
                other.m21 == m21 &&
                other.m22 == m22 &&
                other.m23 == m23 &&

                other.m30 == m30 &&
                other.m31 == m31 &&
                other.m32 == m32 &&
                other.m33 == m33;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Matrix4 is equal to this Matrix4 instance.
        /// </summary>
        /// <param name="other">The Matrix4 to compare this instance to.</param>
        /// <returns>True if the other Matrix4 is equal to this instance; False otherwise.</returns>
        public bool Equals(Matrix4 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this Matrix4 instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this Matrix4; False otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix4))
                return false;

            return Equals((Matrix4)obj);
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return
                m00.GetHashCode() + m01.GetHashCode() + m02.GetHashCode() + m03.GetHashCode() +
                m10.GetHashCode() + m11.GetHashCode() + m12.GetHashCode() + m13.GetHashCode() +
                m20.GetHashCode() + m21.GetHashCode() + m22.GetHashCode() + m23.GetHashCode() +
                m30.GetHashCode() + m31.GetHashCode() + m32.GetHashCode() + m33.GetHashCode();
        }

        public override string ToString()
        {
            string str1 = "Matrix4(\n";
            for (int index1 = 0; index1 < 4U; ++index1)
            {
                string str2 = str1 + " (";
                for (int index2 = 0; index2 < 4U; ++index2)
                    str2 += string.Format("{0:0.0000} ", this[index1, index2]);
                str1 = str2 + ")\n";
            }
            return str1 + ")";
        }

        public Matrix4 Clone()
        {
            return new Matrix4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
        }
    }
}
