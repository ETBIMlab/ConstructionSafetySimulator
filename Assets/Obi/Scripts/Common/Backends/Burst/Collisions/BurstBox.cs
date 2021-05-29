#if (OBI_BURST && OBI_MATHEMATICS && OBI_COLLECTIONS)
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

namespace Obi
{
    public struct BurstBox : BurstLocalOptimization.IDistanceFunction, IBurstCollider
    {
        public BurstColliderShape shape;
        public BurstAffineTransform transform;
        public float dt;

        public void Evaluate(float4 point, ref BurstLocalOptimization.SurfacePoint projectedPoint)
        {
            float4 center = shape.center * transform.scale;
            float4 size = shape.size * transform.scale * 0.5f;

            // clamp the point to the surface of the box:
            point = transform.InverseTransformPointUnscaled(point) - center;

            if (shape.is2D != 0)
                point[2] = 0;

            // get minimum distance for each axis:
            float4 distances = size - math.abs(point);

            if (distances.x >= 0 && distances.y >= 0 && distances.z >= 0)
            {
                // find minimum distance in all three axes and the axis index:
                float min = float.MaxValue;
                int axis = 0;

                for (int i = 0; i < 3; ++i)
                {
                    if (distances[i] < min)
                    {
                        min = distances[i];
                        axis = i;
                    }
                }

                projectedPoint.normal = float4.zero;
                projectedPoint.point = point;

                projectedPoint.normal[axis] = point[axis] > 0 ? 1 : -1;
                projectedPoint.point[axis] = size[axis] * projectedPoint.normal[axis];
            }
            else
            {
                projectedPoint.point = math.clamp(point, -size, size);
                projectedPoint.normal = math.normalizesafe(point - projectedPoint.point);
            }

            projectedPoint.point = transform.TransformPointUnscaled(projectedPoint.point + center + projectedPoint.normal * shape.contactOffset);
            projectedPoint.normal = transform.TransformDirection(projectedPoint.normal);
        }

        public void Contacts(int colliderIndex,

                              NativeArray<float4> positions,
                              NativeArray<float4> velocities,
                              NativeArray<float4> radii,

                              NativeArray<int> simplices,
                              in BurstAabb simplexBounds,
                              int simplexIndex,
                              int simplexStart,
                              int simplexSize,

                              NativeQueue<BurstContact>.ParallelWriter contacts,
                              int optimizationIterations,
                              float optimizationTolerance)
        {
            var co = new BurstContact() { bodyA = simplexIndex, bodyB = colliderIndex };
            float4 simplexBary = BurstMath.BarycenterForSimplexOfSize(simplexSize);

            var colliderPoint = BurstLocalOptimization.Optimize<BurstBox>(ref this, positions, radii, simplices, simplexStart, simplexSize,
                                                        ref simplexBary, out float4 convexPoint, optimizationIterations, optimizationTolerance);

            co.pointB = colliderPoint.point;
            co.normal = colliderPoint.normal;
            co.pointA = simplexBary;

            contacts.Enqueue(co);
        }

        /*public static void Contacts(int particleIndex,
                                    float4 position,
                                    quaternion orientation,
                                    float4 radii,
                                    int colliderIndex,
                                    BurstAffineTransform transform,
                                    BurstColliderShape shape,
                                    NativeQueue<BurstContact>.ParallelWriter contacts)
        {
            BurstContact c = new BurstContact()
            {
                entityA = particleIndex,
                entityB = colliderIndex,
            };

            float4 center = shape.center * transform.scale;
            float4 size = shape.size * transform.scale * 0.5f;

            position = transform.InverseTransformPointUnscaled(position) - center;

            // Get minimum distance for each axis:
            float4 distances = size - math.abs(position);

            // if we are inside the box:
            if (distances.x >= 0 && distances.y >= 0 && distances.z >= 0)
            {
                // find minimum distance in all three axes and the axis index:
                float min = float.MaxValue;
                int axis = 0;
                for (int i = 0; i < 3; ++i)
                {
                    if (distances[i] < min)
                    {
                        min = distances[i];
                        axis = i;
                    }
                }

                c.normal = float4.zero;
                c.point = position;

                c.distance = -distances[axis];
                c.normal[axis] = position[axis] > 0 ? 1 : -1;
                c.point[axis] = size[axis] * c.normal[axis];
            }
            else // we are outside the box:
            {
                // clamp point to be inside the box:
                c.point = math.clamp(position, -size, size);

                // find distance and direction to clamped point:
                float4 diff = position - c.point;
                c.distance = math.length(diff);
                c.normal = diff / (c.distance + math.FLT_MIN_NORMAL);
            }

            c.point += center;
            c.point = transform.TransformPointUnscaled(c.point);
            c.normal = transform.TransformDirection(c.normal);

            c.distance -= shape.contactOffset + BurstMath.EllipsoidRadius(c.normal, orientation, radii.xyz);

            contacts.Enqueue(c);
        }*/

    }

}
#endif