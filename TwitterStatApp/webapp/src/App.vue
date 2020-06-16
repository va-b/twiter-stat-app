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
                <v-simple-table dense class="elevation-4 mytable"  style="flex-basis: 100%">
                    <thead>
                    <template>
                        <tr>
                            <th rowspan="2">Время</th>
                            <th colspan="3" v-if="!stats.length">Пользователь</th>
                            <template v-else>
                                <th colspan="3"  v-for="item in stats">{{item.userName}}</th>
                            </template>
                        </tr>
                        <tr>
                            <template v-if="stats.length === 0">
                                <th>Лайков</th>
                                <th>Твитов</th>
                                <th>Медиана</th>
                            </template>
                            <template v-else v-for="item in stats">
                                <th>Лайков</th>
                                <th>Твитов</th>
                                <th>Медиана</th>
                            </template>
                        </tr>
                    </template>

                    </thead>
                    <tbody>
                    <tr v-if="stats.length && !zeroHours.includes(n - 1)" v-for="n in 24">
                        <td style="white-space: nowrap">
                            {{new Date(2020, 1, 1, n-1, 0, 0, 0).toLocaleTimeString('ru-RU', {hour: '2-digit',
                            minute:'2-digit'})}}
                            -
                            {{new Date(2020, 1, 1, n, 0, 0, 0).toLocaleTimeString('ru-RU', {hour: '2-digit',
                            minute:'2-digit'})}}
                        </td>
                        <template v-for="item in stats">
                            <td class="text-right" :style="getCellStyle(item.userName, n-1)">
                                {{item.likesTimeRange[n-1].toLocaleString()}}
                            </td>
                            <td class="text-right" :style="getCellStyle(item.userName, n-1)">
                                {{item.tweetsTimeRange[n-1].toLocaleString()}}
                            </td>
                            <td class="text-right" :style="getCellStyle(item.userName, n-1)">
                                {{item.medianTimeRange[n-1].toLocaleString()}}
                            </td>
                        </template>
                    </tr>
                    </tbody>
                    <tfoot class="font-weight-medium blue--text text--darken-2" v-if="stats.length">
                    <tr>
                        <th>Итого</th>
                        <template v-for="item in stats">
                            <th class="text-right">{{item.likesTimeRange[24].toLocaleString()}}</th>
                            <th class="text-right">{{item.tweetsTimeRange[24].toLocaleString()}}</th>
                            <th class="text-right">{{item.medianTimeRange[24].toLocaleString()}}</th>
                        </template>
                    </tr>
                    </tfoot>
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
        search: string | null, 
        stats: TweetStatisticModel[]
    },{}, {
        zeroHours: number[];
        bestTimeForUsers: {[userName in string]: number};
        worstTimeForUsers: {[userName in string]: number};
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
            zeroHours()
            {
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
            bestTimeForUsers()
            {
                let res: any = {};
                if (this.stats.length > 0 ) this.stats.forEach(x => {
                    let max = Math.max(...x.medianTimeRange.slice(0, x.medianTimeRange.length - 1));
                    res[x.userName] = x.medianTimeRange.indexOf(max);
                });
                return res;
            },
            worstTimeForUsers()
            {
                let res: any = {};
                if (this.stats.length > 0 ) this.stats.forEach(x => {
                    let min = Math.min(...x.medianTimeRange.slice(0, x.medianTimeRange.length - 1).filter(y => y != 0));
                    res[x.userName] = x.medianTimeRange.indexOf(min);
                });
                return res;
            }
        },
        methods: {
            getCellStyle(user: string, hour: number)
            {
                if(this.worstTimeForUsers[user] == hour) return {backgroundColor: '#ffa99e'}
                else if (this.bestTimeForUsers[user] == hour) return {backgroundColor: '#a2ff9e'}
                else return {}
            }
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
<style>
    .mytable th, td
    {
        border-left: 1px solid #aaa !important;
        border-bottom: 1px solid #aaa !important;
    }
</style>
