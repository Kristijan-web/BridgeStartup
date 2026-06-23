import { Routes } from '@angular/router';
import { HomePage } from './pages/home-page';
import { UserLayout } from './layouts/user/user-layout';
import { AboutUs } from './pages/about-us';

export const routes: Routes = [
    {
        path: "",
        component: UserLayout, 
        children: [
            {
                path: "",
                component: HomePage, 
                title: 'BridgeStartup'

            },
            {
                
                path: "about",
                component: AboutUs,
                title: 'About us'
            }
        ]
    }
];


