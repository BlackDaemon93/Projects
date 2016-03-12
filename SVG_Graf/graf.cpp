#include "graf.h"
#include <cstdlib>
#include <ctime>
#include <cmath>
#define M_PI 3.14159265358979323846
struct Muchie {
    int v;
    Muchie *next;
    };
struct Nod
    {
        unsigned alocat:1;
        int x,y;
        Muchie *first;
    };
class LCircle:public Layout {
    int generate() {
        if(!currentGraf)
            {
            cout << "Eroare de generare: nu exista graf asociat\n";
            return 0;
            }
        int maxno = currentGraf->number_of_vertices();
        if(maxno == 0) return 0;
        int k;
        double radius = 384,angle = (2*M_PI) / maxno,cangle,centerx,centery,cx,cy;
        if(radius*sin(angle/2)<44.0)
            radius = 44 / sin(angle/2);
        centerx = radius + 37;
        centery = radius + 37;
        cangle = 0;
        for(k=0,cangle=0;k<maxno;k++,cangle += angle)
        {
            cx = centerx + radius * sin(cangle);
            cy = centery + radius * cos(cangle);
            currentGraf->putCoord(k,(int)cx,(int)cy);
        }
        return 0;
        }
    };
/* Generare aleatoare, calcul corrdonate:
int cx = lnod[cap->v].x*80+40;
int cy = lnod[cap->v].y*80+40; */
// maxima pe orizontala
const int maxh = 20;
class LRandom:public Layout {
    int generate()
        {
            if(!currentGraf)
                {
                cout << "Eroare de generare: nu exista graf asociat\n";
                return 0;
                }
            int maxno = currentGraf->number_of_vertices(),k,u;
            if(maxno == 0) return 0;
            srand(time(NULL));
            int *px = new int [maxno], *py = new int [maxno];
            // maxh*maxv >= 2*maxno => maxv>=3*maxno/maxh
            int maxv = (3*maxno)/maxh;
            if(maxv < maxh) maxv = maxh;
            k=0;
            do {
                px[k] = rand() % maxh;
                py[k] = rand() % maxv;
                for(u=0;u<k;u++)
                    if(px[u]==px[k] && py[u]==py[k]) break;
                if(u==k) k++;
                } while(k<maxno);
            for(k=0;k<maxno;k++)
                currentGraf->putCoord(k,px[k]*80+40,py[k]*80+40);
            delete [] px; delete [] py;
            return 1;
        }
    };
static LCircle cCircle;
static LRandom cRandom;

Layout *lcircle = &cCircle,*lrandom = &cRandom;

void StergLista(Muchie *&p)
    {
        Muchie *cnext;
        while(p)
        {
            cnext = p->next;
            delete p;
            p = cnext;
        }
    }
void StergLista(Muchie *&cap,int val)
    {
        Muchie *ant,*c;
        for(ant=0,c=cap;c && c->v != val;ant = c,c=c->next);
        if(!c)
        {
            cout << "Graf,StergLista:Eroare de programare\n";
            exit(1);
        }
        if(!ant) cap = c->next;
        else ant->next = c->next;
        delete c;
    }
int CautaLista(Muchie *cap,int val)
    {
        while(cap)
            if(cap->v == val) return 1;
            else cap = cap->next;
        return 0;
    }
void ListaCreste(Nod *&lnod,int &capacitate,int newCap)
    {
    int oldcap = capacitate;
    while(capacitate<newCap) capacitate *= 2;
    Nod *newList = new Nod [capacitate];
    for(int k=0;k<capacitate;k++)
        if(k<oldcap)
            newList[k] = lnod[k];
        else
            {
                newList[k].alocat=0;
                newList[k].first = 0;
            }
    delete [] lnod;
    lnod = newList;
    }
Nod& Graf::getNode(int index)
    {
        int v=0;
        do {
            if(lnod[v].alocat)
                {
                if(!index) return lnod[v];
                else index--;
                }
            v++;
            } while(1);
    }

void Graf::putCoord(int indexNod,int cx,int cy)
    {
        Nod& cnode = getNode(indexNod);
        cnode.x = cx; cnode.y = cy;
    }
int Graf::automatic_layout()
    {
    currentLayout = lrandom;
    if(n>0)
        {
        currentLayout->setGraf(this);
        currentLayout->generate();
        return 1;
        }
    return 0;
    }
int Graf::applyLayout(Layout *playout)
    {
    if(currentLayout != playout)
        currentLayout = playout;
    if(currentLayout && n>0)
        {
        currentLayout->setGraf(this);
        currentLayout->generate();
        return 1;
        }
    return 0;
    }
Graf::Graf()
            {
            n=m=0;
            capacitate = 32;
            lnod = new Nod [capacitate];
            for(int k=0;k<capacitate;k++)
                { lnod[k].alocat=0; lnod[k].first = 0; }
            currentLayout = lrandom;
            }
bool Graf::exists_node(int id)
            {
                if(id>=capacitate) return false;
                else return (lnod[id].alocat == 1);
            }
void Graf::add_node(int id)
            {
                if(id>=capacitate)
                        ListaCreste(lnod,capacitate,id+1);
                if(exists_node(id))
                {
                    cout << "Graf,add_node: nodul exista deja\n";
                    return;
                }
                lnod[id].alocat = 1;
                n++;
            }
void Graf::remove_node(int id)
            {
                if(!exists_node(id))
                {
                    cout << "Graf,remove:nodul " << id << " nu a fost alocat\n";
                    return;
                }
                Muchie *c = lnod[id].first;
                while(c)
                {
                    StergLista(lnod[c->v].first,id);
                    c = c->next;
                }
                StergLista(lnod[id].first);
                lnod[id].alocat = 0;
                n--;
            }
bool Graf::exists_edge(int id1,int id2)
            {
                return CautaLista(lnod[id1].first,id2)==1;
            }
void Graf::add_edge(int id1,int id2)
            {
                if(!exists_node(id1) || !exists_node(id2))
                {
                    cout << "Graf,add_edge: noduri capete nealocate\n";
                    return;
                }
                if(exists_edge(id1,id2))
                {
                    cout << "Graf,add_edge: muchia exista deja\n";
                    return;
                }
                Muchie *pnew;;
                pnew = new Muchie; pnew->v = id2;
                pnew->next = lnod[id1].first;
                lnod[id1].first=pnew;
                pnew = new Muchie; pnew->v = id1;
                pnew->next = lnod[id2].first;
                lnod[id2].first=pnew;
                m++;
            }
void Graf::remove_edge(int id1,int id2)
            {
                if(!exists_edge(id1,id2))
                {
                    cout << "Graf,remove_edge:muchia nu exista\n";
                    return;
                }
                StergLista(lnod[id1].first,id2);
                StergLista(lnod[id2].first,id1);
                m--;
            }
void Graf::read(istream &in)
    {
        int k,no,me,v1,v2;
        n=m=0;
        in >> no >> me;
        if(no==0) { return; }
        for(k=0;k<no;k++)
        {
            in >> v1;
            add_node(v1);
        }
        for(k=0;k<me;k++)
        {
            in >> v1 >> v2;
            add_edge(v1,v2);
        }
    }
void Graf::print(ostream &out)
            {
                if(n==0) { out << "Graf vid\n"; return; }
                int v;
                if(empty())
                    {
                        out << "Graf gol\n";
                        out << "Varfuri:";
                        for(v=0;v<capacitate;v++)
                            if(lnod[v].alocat) out << v << ' ';
                        out << '\n';
                        return;
                    }
                Muchie *c;
                out << n << ' ' << m << endl;
                for(v=0;v<capacitate;v++)
                    if(lnod[v].alocat) out << v << ' ';
                out << '\n';
                for(v=0;v<capacitate;v++)
                    if(lnod[v].alocat)
                        for(c=lnod[v].first;c;c=c->next)
                            if(c->v>v) out << v <<' '<<c->v<<endl;
            }
Graf::~Graf()
            {
                if(m) // daca exista muchii
                {
                    // sterg toate listele de muchii alocate:
                    for(int k=0;k<capacitate;k++)
                        if(lnod[k].alocat && lnod[k].first)
                            StergLista(lnod[k].first);
                }
                // sterg lista de noduri:
                delete [] lnod;
            }
char head_svg[]="<?xml version=\"1.0\" standalone=\"no\" ?>\n\
<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n";
/* <svg width="16cm" height="4cm" viewBox="0 0 1600 400"
        xmlns="http://www.w3.org/2000/svg" version="1.1"> */
char svg_end[]="</svg>\n";
void put_viewbox(ostream &out, int cx, int cy)
{
    out << "<svg width=\""<<cx<<"cm\" height=\""<<cy<<"cm\" viewBox=\"0 0 "<<cx*100<<' '<<cy*100<<"\" ";
    out <<  "xmlns=\"http://www.w3.org/2000/svg\" version=\"1.1\">\n";
}
void draw_edge(ostream &out, int cx, int cy,int cx1, int cy1)
{
    out << "\t<line x1=\""<<cx<<"\" y1=\""<<cy<<"\" x2=\""
        <<cx1<<"\" y2=\""<<cy1<<"\" stroke-width=\"2\" stroke=\"black\" />\n";
}
void draw_node(ostream &out,int v,int cx, int cy)
{
    out << "\t<circle cx=\""<<cx<<"\" cy=\""<<cy<<"\" r=\"32\" fill=\"rgb(255,0,0)\"/>\n";
    out << "\t<text x=\""<<cx<<"\" y=\""<<cy+12<<"\" font-size=\"40\" text-anchor=\"middle\" fill=\"black\">"
        <<v<<"</text>\n";
}
int Graf::save_svg(ostream &out)
    {
        if(currentLayout && n>0)
            {
                out << head_svg;
                int maxx = 0,maxy=0,v,cx,cy,cindex;
                for(cindex=0;cindex<n;cindex++)
                {
                    Nod& cnode = getNode(cindex);
                    if(maxx < cnode.x) maxx = cnode.x;
                    if(maxy < cnode.y) maxy = cnode.y;
                }
                maxx += 40; maxy += 40;
                cx = (maxx / 100) + (maxx % 100 ? 1 : 0);
                cy = (maxy / 100) + (maxy % 100 ? 1 : 0);
                put_viewbox(out,cx,cy);
                for(v=0;v<capacitate;v++)
                    if(lnod[v].alocat)
                        {
                            Muchie *cap;
                            cx = lnod[v].x; cy = lnod[v].y;
                            for(cap=lnod[v].first;cap;cap=cap->next)
                                if(cap->v > v)
                                    draw_edge(out,cx,cy,lnod[cap->v].x,lnod[cap->v].y);
                        }
                for(v=0;v<capacitate;v++)
                    if(lnod[v].alocat)
                            draw_node(out,v,lnod[v].x,lnod[v].y);
                out << svg_end;
                return 1;
            }
        else return 0;
    }
