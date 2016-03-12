#ifndef __GRAF_H__
    #define __GRAF_H__
#include <iostream>
using namespace std;
struct Nod;
class Graf;
class Layout {
    protected:
        Graf *currentGraf;
    public:
        virtual int generate() = 0;
        void setGraf(Graf *pGraf) { currentGraf = pGraf; }
    };
class Graf {
    int n,m,capacitate;
    Nod *lnod;
    Layout *currentLayout;
    Nod& getNode(int index);
    public:
        Graf();
        int automatic_layout();
        int applyLayout(Layout *playout);
        bool exists_node(int id);
        void add_node(int id);
        void remove_node(int id);
        bool exists_edge(int id1,int id2);
        void add_edge(int id1,int id2);
        void remove_edge(int id1,int id2);
        void putCoord(int indexp,int cx,int cy);
        int number_of_vertices() { return n; } // inline
        int number_of_edges() { return m; } // inline
        // Un graf l-am considerat gol, daca numarul de muchii este 0
        bool empty() { return m==0; } // inline
        void print(ostream &out);
        void read(istream &in);
        int save_svg(ostream &out);
        ~Graf();
    };
extern Layout *lcircle,*lrandom;
#endif // __GRAF_H__
