This is a sample application of the 2D testbed, documented further in the "Getting started" guide. 
An small Mona Lisa image is loaded and displayed. Adjusting a slider, the user can "destroy" some fraction of the pixels (the destroyed ones are shown in red in the "damaged" image). From the damaged image (and a knowlege of WHICH pixels are damaged, but not the original values) a repaired image is constructed, by saying that each damaged pixel should be replaced with the average of its neighbors, repeatedly. 

Whenever the damage-fraction is adjusted, the repaired image is reset. 