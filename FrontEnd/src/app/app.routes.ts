import { Routes } from '@angular/router';
import { HomePage } from './pages/home-page';
import { UserLayout } from './layouts/user/user-layout';
import { AboutUsPage } from './pages/about-us-page';
import { ContactPage } from './pages/contact-page';
import { PostDetailsPage } from './pages/post-details-page';

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
                component: AboutUsPage,
                title: 'About us'
            },
            {
                path: "contact",
                component: ContactPage,
                title: "Contact us"
            }, 
            {
                path: "posts/:id",
                component: PostDetailsPage,
            title: "Post details"
            }
        ]
    }
];


