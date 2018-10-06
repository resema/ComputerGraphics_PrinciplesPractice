A program to demonstrate corner-cutting subdivision: at the start, the user can click several points to form 
a closed polygon. When the user clicks the "Subdivide" button, the first and last third of each edge are 
removed, leaving the middle third of each edge. Adjacent end points of these thirds are joined, forming a new 
closed polygon with "cut off corners". Repeated corner cutting leads to polygons that appear somewhat smoother. 

You could add a slider and experiment with how much of each edge is retained (lines 93-94 and 97-98) to see whether 
the results can be made smoother. 
