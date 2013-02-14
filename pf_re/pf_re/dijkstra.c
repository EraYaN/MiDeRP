#include <stdlib.h>
#include <stdio.h>

#include "dijkstra.h"
#include "util.h"
#include "line.h"
#include "node.h"

Node *getNode(int x, int y){
    return nodes[y*n+x];
}

///     |
///  -------
/// _|  |  |_ n
///  |  |  |
///  -------
///     | m

Node *getNodeFromControlPost(int controlPost)
{
    //TODO
    int i;
    const int hori = m-2; //controlPosts per horizontale zijde van het veld
    const int vert = n-2; //controlPosts per verticale zijde van het veld
    if(controlPost > numControlPosts){
        //bestaat niet!
        return NULL;
    }
    if(controlPost<=hori){
        //onder
        return getNode(controlPost,0);
    } else if(controlPost>hori||controlPost<=hori+vert){
        //rechts
        return getNode(m,controlPost-hori);
    } else if(controlPost>hori+vert||controlPost<=2*hori+vert){
        //boven
        return getNode((2*hori+vert)-(controlPost-hori-vert),n);
    } else if(controlPost>2*hori+vert||controlPost<=2*hori+2*vert){
        //links
        return getNode(0,((2*hori+2*vert)-(controlPost-2*hori-vert)));
    } else {
        //bestaat niet!
        return NULL;
    }
    for(i=0;i<numControlPosts;i++){

    }
    return NULL;
}
Line *getLine(Node *origin, Node *destination){
    int i;
    for(i=0; i<numLines; i++){
        if(lines[i]->origin == origin && lines[i]->destination == destination && lines[i]->mine == 0)
            return lines[i];
    }
    return NULL;
}
