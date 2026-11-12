using BubbleBuffs.Handlers;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic.Commands;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BubbleBuffs {
    public class AnimatedExecutionEngine : IBuffExecutionEngine {
        private UnitCommand Cast(CastTask task) {
            try {
                // Subscribe to the RuleCastSpell event that will be executed by the cast command
                EventBus.Subscribe(new EngineCastingHandler(task));

                // Return the command that uses animation for casting
                return UnitUseAbility.CreateCastCommand(task.SpellToCast, task.Target);
            } 
            catch (Exception ex) {
                Main.Error(ex, "Animated Engine Casting");
                return null;
            }
        }

        public IEnumerator CreateSpellCastRoutine(List<CastTask> tasks) {
            var byCaster = tasks.GroupBy(task => task.Caster).Select(x => x.GetEnumerator()).ToList();
            UnitCommand[] running = new UnitCommand[byCaster.Count];

            // The Extend rods of each caster, set for the command it is running. The command
            // creates and triggers the cast rule at some point of its animation, so the rods
            // stay set until the command is over. A caster runs one command at a time.
            IDisposable[] heldRods = new IDisposable[byCaster.Count];

            int remaining = byCaster.Count;

            while (byCaster.Any(x => x != null)) {

                for (int i = 0; i < byCaster.Count; i++) {
                    var current = running[i];
                    if (current != null) {
                        if (current.IsFinished) {
                            running[i] = null;
                            heldRods[i]?.Dispose();
                            heldRods[i] = null;
                        }
                        continue;
                    }


                    var queue = byCaster[i];
                    if (queue == null) {
                        continue;
                    }

                    if (!queue.MoveNext()) {
                        byCaster[i] = null;
                        continue;
                    }

                    current = Cast(queue.Current);
                    heldRods[i] = ExtendRods.Hold(queue.Current.Caster, queue.Current.ExtendRod);
                    queue.Current.Caster.Commands.Run(current);
                    running[i] = current;
                    break;
                }

                yield return new WaitForFixedUpdate();

            }
            yield return null;
        }
    }
}
