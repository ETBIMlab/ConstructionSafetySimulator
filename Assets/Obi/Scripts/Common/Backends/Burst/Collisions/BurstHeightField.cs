﻿#if (OBI_BURST && OBI_MATHEMATICS && OBI_COLLECTIONS)
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

namespace Obi
{
    public struct BurstHeightField : BurstLocalOptimization.IDistanceFunction, IBurstCollider
    {

        public BurstColliderShape shape;
        public BurstAffineTransform transform;
        public float dt;

        public BurstMath.CachedTri tri;
        public float4 triNormal;

        public HeightFieldHeader header;
        public NativeArray<float> heightFieldSamples;

        public void Evaluate(float4 point, ref BurstLocalOptimization.SurfacePoint projectedPoint)
        {
            point = transform.InverseTransformPoint(point);

            float4 nearestPoint = BurstMath.NearestPointOnTri(tri, point, out float4 bary);
            float4 normal = math.normalizesafe(point - nearestPoint);

            // flip the contact normal if it points below ground:
            BurstMath.OneSidedNormal(triNormal, ref normal);

            projectedPoint.point = transform.TransformPoint(nearestPoint + normal * shape.contactOffset);
            projectedPoint.normal = transform.TransformDirection(normal);
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
            if (shape.dataIndex < 0) return;

            triNormal = float4.zero;

            var co = new BurstContact() { bodyA = simplexIndex, bodyB = colliderIndex };

            int resolutionU = (int)shape.center.x;
            int resolutionV = (int)shape.center.y;

            // calculate terrain cell size:
            float cellWidth = shape.size.x / (resolutionU - 1);
            float cellHeight = shape.size.z / (resolutionV - 1);

            // calculate particle bounds min/max cells:
            int2 min = new int2((int)math.floor(simplexBounds.min[0] / cellWidth), (int)math.floor(simplexBounds.min[2] / cellHeight));
            int2 max = new int2((int)math.floor(simplexBounds.max[0] / cellWidth), (int)math.floor(simplexBounds.max[2] / cellHeight));

            for (int su = min[0]; su <= max[0]; ++su)
            {
                if (su >= 0 && su < resolutionU - 1)
                {
                    for (int sv = min[1]; sv <= max[1]; ++sv)
                    {
                        if (sv >= 0 && sv < resolutionV - 1)
                        {
                            // calculate neighbor sample indices:
                            int csu1 = math.clamp(su + 1, 0, resolutionU - 1);
                            int csv1 = math.clamp(sv + 1, 0, resolutionV - 1);

                            // sample heights:
                            float h1 = heightFieldSamples[header.firstSample + sv * resolutionU + su] * shape.size.y;
                            float h2 = heightFieldSamples[header.firstSample + sv * resolutionU + csu1] * shape.size.y;
                            float h3 = heightFieldSamples[header.firstSample + csv1 * resolutionU + su] * shape.size.y;
                            float h4 = heightFieldSamples[header.firstSample + csv1 * resolutionU + csu1] * shape.size.y;

                            float min_x = su * shape.size.x / (resolutionU - 1);
                            float max_x = csu1 * shape.size.x / (resolutionU - 1);
                            float min_z = sv * shape.size.z / (resolutionV - 1);
                            float max_z = csv1 * shape.size.z / (resolutionV - 1);

                            float4 convexPoint;
                            float4 simplexBary = BurstMath.BarycenterForSimplexOfSize(simplexSize);

                            // contact with the first triangle:
                            float4 v1 = new float4(min_x, h3, max_z, 0);
                            float4 v2 = new float4(max_x, h4, max_z, 0);
                            float4 v3 = new float4(min_x, h1, min_z, 0);

                            tri.Cache(v1, v2, v3);
                            triNormal.xyz = math.normalizesafe(math.cross((v2 - v1).xyz, (v3 - v1).xyz));

                            var colliderPoint = BurstLocalOptimization.Optimize<BurstHeightField>(ref this, positions, radii, simplices, simplexStart, simplexSize,
                                                                                ref simplexBary, out convexPoint, optimizationIterations, optimizationTolerance);

                            co.pointB = colliderPoint.point;
                            co.normal = colliderPoint.normal;
                            co.pointA = simplexBary;

                            contacts.Enqueue(co);

                            // contact with the second triangle:
                            v1 = new float4(min_x, h1, min_z, 0);
                            v2 = new float4(max_x, h4, max_z, 0);
                            v3 = new float4(max_x, h2, min_z, 0);

                            tri.Cache(v1, v2, v3);
                            triNormal.xyz = math.normalizesafe(math.cross((v2 - v1).xyz, (v3 - v1).xyz));

                            colliderPoint = BurstLocalOptimization.Optimize<BurstHeightField>(ref this, positions, radii, simplices, simplexStart, simplexSize,
                                                                                ref simplexBary, out convexPoint, optimizationIterations, optimizationTolerance);

                            co.pointB = colliderPoint.point;
                            co.normal = colliderPoint.normal;
                            co.pointA = simplexBary;

                            contacts.Enqueue(co);
                        }
                    }
                }
            }
            
        }

    }

}
#endif