/*
 * Copyright (c) 2021 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RayWenderlich.SpaceInvadersUnity
{
    public class InvaderSwarm : MonoBehaviour
    { 
        [System.Serializable]
       private struct invaderType
       {
           public string name;
           public Sprite[] sprites;
           public int points;
           public int rowCount;
       }
       [SerializeField] private invaderType[] invaderSwarms;
       [SerializeField] private int columnCounts = 11;
       [SerializeField] private int xSpacing;
       [SerializeField] private int ySpacing;
       [SerializeField] private Transform spawnStartpoint;
       private float minX;
       private float maxX;
       private float currentX;
       private float xIncreament;
       private bool isMovingRight = true;
       private int rowCount;
       private Transform[,] invaders;
       [SerializeField]
       private float speedFactor = 10f;
       internal static InvaderSwarm Instance;
       [SerializeField] private BulletSpawner bulletSpawnerPrefab;
       private int killCount;
       private System.Collections.Generic.Dictionary<string, int> pointsMap;
       
       
       private void Start()
       {
           SpawnSwarm();
           for (int i = 0; i < columnCounts; i++)
           {
               var bulletSpawner = Instantiate(bulletSpawnerPrefab, transform.position, Quaternion.identity);
               bulletSpawner.column = i;
               bulletSpawner.currentRow = rowCount - 1;
               bulletSpawner.Setup();
           }
       }
       
       private void Awake()
       {
           if (Instance == null)
           {
               Instance = this;
           }
           else if(Instance != this)
           {
               Destroy(gameObject);
           }
       }

       public void SpawnSwarm()
       {
           minX = spawnStartpoint.position.x;
           GameObject swarm = new GameObject() { name = "Swarm" };

           Vector2 currentPos = spawnStartpoint.position;
           foreach (var invaderType in invaderSwarms)
           {
               rowCount += invaderType.rowCount;
           }

           maxX = minX + 2f * xSpacing * columnCounts;
           currentX = minX;
           invaders = new Transform[rowCount, columnCounts];
           int rowIndex = 0;

           pointsMap = new Dictionary<string, int>();
           foreach (var invaderType in invaderSwarms)
           {
               var invaderName = invaderType.name;

               for (int i = 0; i < invaderType.rowCount; i++)
               {
                   for (int j = 0; j < columnCounts; j++)
                   {
                       GameObject invader = new GameObject() { name = invaderName };
                       invader.AddComponent<SimpleAnimator>().sprites = invaderType.sprites;
                       invader.transform.position = currentPos;
                       invader.transform.SetParent(swarm.transform);
                       pointsMap[invaderName] = invaderType.points;
                       invaders[rowIndex, j] = invader.transform;
                       currentPos.x += xSpacing;
                   }
        
                   currentPos.x = minX;
                   currentPos.y -= ySpacing;

                   rowIndex++;
               }
           }
       }
       
       
       private void Update()
       {
           Move();
       }

       private void Move()
       {
           xIncreament = speedFactor * Time.deltaTime;
           if (isMovingRight)
           {
               currentX += xIncreament;
               if (currentX < maxX)
               {
                   MoveInvader(xIncreament, 0);
               }
               else
               {
                   ChangeDirection();
               }
           }
           else
           {
               currentX -= xIncreament;
               if (currentX > minX)
               {
                   MoveInvader(-xIncreament, 0);
               }
               else
               {
                   ChangeDirection();
               }
           }
       }

       private void MoveInvader(float x, float y)
       {
           for (int i = 0; i < rowCount; i++)
           {
               for (int j = 0; j < columnCounts; j++)
               {
                   invaders[i, j].Translate(x, y, 0);
               }
           }
       }

       public void ChangeDirection()
       {
           isMovingRight = !isMovingRight;
           MoveInvader(0, -ySpacing);
       }

       internal Transform GetInvader(int row, int column)
       {
           if (row < 0 && column < 0 || row > invaders.GetLength(0) || column > invaders.GetLength(1))
           {
               return null;
           }

           return invaders[row, column];
       }

       internal void IncreaseDeadthCount()
       {
           killCount++;
           if (killCount > invaders.Length)
           {
               GameManager.Instance.TriggerGameOver(false);
           }
       }

       internal int GetPoint(string AlienName)
       {
           if (pointsMap.ContainsKey(AlienName))
           {
               return pointsMap[AlienName];
           }

           return 0;
       }

       
    }
}