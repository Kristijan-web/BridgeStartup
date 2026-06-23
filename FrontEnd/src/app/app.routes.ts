import { Routes } from '@angular/router';
import { HomePage } from './pages/home-page';
import { UserLayout } from './layouts/user/user-layout';
import { PostsPage } from './pages/posts-page';

export const routes: Routes = [
    {
        path: "",
        component: UserLayout, 
        children: [
            {
                path: "",
                component: HomePage, 

            },
            {
                
                path: "posts",
                component: PostsPage
            }
        ]
    }
];


// Od cega zavisi da li cu vratiti user layout ili admin layout?
// - Putanja

// Ako korisnik pokusa da pristupi /admin/dashboard onda se vraca skroz drugi layout, ali da bi uopste pristupio toj ruti radi se autorizacija


// Kako da napisem da kada se pogodi / ruta da se vrati user layout i onda kada se bilo sta promeni unutar te rute  da se samo promeni <outlet> deo