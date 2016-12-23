CSOrbits

This came about following a discussion about an old university project to create 2 circles in space and simulate the effects
of gravity on them.

I created code to do this and it was rather boring, so I added a lot more balls and then added collision detection and rebounds.

The maths behind this turned out to be surprisingly complex.


The simulation is set up to start with a large object and 3 smaller objects orbiting it on the left, a large group of stationary objects of random sizes on the right and a high speed object coming in from offscreen on the right. Takes a while for the high speed object to arrive, so you get time to see the gravitational interactions before everything goes ricocheting around.
