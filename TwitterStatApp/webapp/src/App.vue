<template>
    <v-app>
        <v-main>
            <v-container fluid class="blue lighten-3 fill-height justify-center align-content-start"
                         style="flex-wrap: wrap"
            >
                <v-sheet color="white" width="600px" elevation="4" class="pa-3 mb-4">
                    <v-combobox
                            v-model="selectedUsers"
                            :items="users"
                            cache-items
                            :loading="isUsersLoading"
                            :search-input.sync="search"
                            class="mx-auto mb-4"
                            label="Имя пользователя"
                            placeholder="type to search, press enter to add"
                            hide-details
                            multiple
                            chips
                            clearable
                            deletable-chips
                    />
                </v-sheet>
                <v-simple-table dense class="elevation-4"  style="flex-basis: 100%">
                    <thead>
                    <tr>
                        <th style="white-space: nowrap">Пользователь</th>
                        <th 
                                class="lighten-3"
                                v-for="n in 24" 
                                v-if="stats.length === 0 || summary[n-1] !== 0" 
                                style="white-space: nowrap"
                                :class="n-1 === bestTime ? 'success' : n-1 === worstTime ? 'error' : ''"
                        >
                            {{new Date(2020, 1, 1, n-1, 0, 0, 0).toLocaleTimeString('ru-RU', {hour: '2-digit',
                            minute:'2-digit'})}}
                            -
                            {{new Date(2020, 1, 1, n, 0, 0, 0).toLocaleTimeString('ru-RU', {hour: '2-digit',
                            minute:'2-digit'})}}
                        </th>
                        <th style="white-space: nowrap">Всего лайков</th>
                        <th style="white-space: nowrap">Всего твитов</th>
                        <th style="white-space: nowrap">Медиана</th>
                    </tr>
                    </thead>
                    <tbody>
                    <tr v-for="item in stats" :key="item.userName">
                        <td>{{item.userName}}</td>
                        <td 
                                class="text-right lighten-3" 
                                v-for="n in 24" 
                                v-if="summary[n-1] !== 0"
                                :class="n-1 === bestTime ? 'success' : n-1 === worstTime ? 'error' : ''"
                        >
                            {{item.likesTimeRange[n-1].toLocaleString()}}
                        </td>
                        <td class="text-right">{{item.totalLikes}}</td>
                        <td class="text-right">{{item.totalTweets}}</td>
                        <td class="text-right lighten-3">
                            {{item.median}}
                        </td>
                    </tr>
                    </tbody>
                </v-simple-table>

            </v-container>
        </v-main>
    </v-app>
</template>

<script lang="ts">
    import Vue from 'vue';
    import TweetStatisticModel from "@/model/TweetStatisticModel";

    export default Vue.extend<{
        selectedUsers: [],
        users: string[],
        isUsersLoading: boolean,
        search: string | null, stats:
            TweetStatisticModel[]
    },
        {},
        {
            // maxmedian: number | null;
            // minmedian: number | null;
            bestTime: number | null;
            worstTime: number | null;
            hoursToIgnore: number[];
            summary: number[];
        }>({
        name: 'App',

        data: () => ({
            selectedUsers: [],
            users: [],
            isUsersLoading: false,
            search: null,
            stats: []
        }),

        computed: {
            summary()
            {
                let res: number[] = [];
                for(let i = 0 ; i < 24; i++) 
                {
                    res.push(this.stats.reduce((a, b) => a + b.likesTimeRange[i], 0));
                }
                return res;
            },
            bestTime()
            {                
                if (this.stats.length == 0) return null;
                let m = Math.max(...this.summary);
                return this.summary.indexOf(m);
            },
            worstTime()
            {
                if (this.stats.length == 0) return null;
                let m = Math.min(...this.summary.filter(x => x != 0));
                return this.summary.indexOf(m);
            },
            // maxmedian() {
            //     if (this.stats.length < 2) return null;
            //     return Math.max(...this.stats.map(x => x.median));
            // },
            // minmedian() {
            //     if (this.stats.length < 2) return null;
            //     return Math.min(...this.stats.map(x => x.median));
            // },
            hoursToIgnore() {
                if (this.stats.length == 0 ) return [];
                
                let hours: number[] = [];
                for(let i = 0; i < 24; i++) 
                {
                    if(this.stats.every(x => x.likesTimeRange[i] == 0))
                    {
                        hours.push(i);
                    }
                }
                return hours;
            },
        },

        watch: {
            async search(val: string) {
                if (this.isUsersLoading) return;
                this.isUsersLoading = true;
                try {
                    this.users = await this.$sf.Tsa.FindUsers(val);
                } catch (e) {
                    console.error(e);
                } finally {
                    this.isUsersLoading = false;
                }
            },
            async selectedUsers(val: string[]) {
                if (val.length === 0) {
                    this.stats = [];
                    return;
                }
                this.stats = await this.$sf.Tsa.GetStatistic(val);
            }
        }
    });
</script>
