import { FilterValue, LabelFilter } from "@components/scores/scores-search/scores-search.component";
import { Category } from "@models/category";
import { Score } from "@models/score";
import { ScoreGroup } from "@models/score-group";

export function normalizeStringForSearch(value: string): string {
    var withoutAccents = value.normalize('NFD').replace(/\p{Diacritic}/gu, '');
    // Trimming improves the UX for the general case.
    // But if someone searches for something with a space at the start or end on purpose, like " 2T",
    // this ignores that space. That experience is degraded, but we think it's a good tradeoff.
    return withoutAccents.toLowerCase().trim();
}

function isEmpty(search: FilterValue): boolean {
    return search.labels.length === 0 && search.text.length === 0;
}


export function filterCategories(search: FilterValue, categories: Category[]): Category[] {
    return filterTree(search, categories);
}

export function filterScoreGroups(search: FilterValue, scoreGroups: ScoreGroup[]): ScoreGroup[] {
    return <ScoreGroup[]>filterTree(search, scoreGroups);
}

function isScoreGroup(item: Category): item is ScoreGroup {
    return item instanceof ScoreGroup;
}

function filterTree(search: FilterValue, items: Category[]): Category[] {
    if (isEmpty(search)) {
        // No filter, just return all.
        return items;
    }
    return items
        .map(item => filterTreeItem(search, item))
        .filter(item => item != null);

}

function filterTreeItem(search: FilterValue, item: Category): Category | null {
    // If there are no labels, we search by title only.
    if (search.labels.length == 0
        // In that case, if the item is a match then we include it with all its children.
        && item.searchableName.includes(search.text)) {
        return item;
    }

    // Otherwise, we filter the children.
    const filteredScores = filterScores(search, item.scores);
    let filteredSubGroups: ScoreGroup[] = [];
    if (isScoreGroup(item)) {
        filteredSubGroups = item.subGroups
            .map(subGroup => <ScoreGroup>filterTreeItem(search, subGroup))
            .filter(subGroup => subGroup != null);
    }

    if (filteredScores.length > 0 || filteredSubGroups.length > 0) {
        if (isScoreGroup(item)) {
            const cloned = new ScoreGroup(item.name, item.parent, filteredScores, filteredSubGroups);
            cloned.subGroups.forEach(child => child.parent = cloned);
            return cloned;
        }
        return new Category(item.name, filteredScores);
    }
    return null;
}

// function filterCategories__OLD(search: FilterValue, categories: Category[]): Category[] {
//     if (isEmpty(search)) {
//         // No filter, just return all.
//         return categories;
//     }
//     return categories
//         .map(category => filterCategory(search, category))
//         .filter(category => category != null);
// }

// function filterCategory(search: FilterValue, category: Category): Category | null {
//     // If there are no labels, we search by title only.
//     if (search.labels.length == 0
//         // In that case, if the category is a match then we include all its children.
//         && category.searchableName.includes(search.text)) {
//         // Filter matches this group, so we return all children.
//         return category;
//     }

//     // Otherwise, we filter the children.
//     const filteredScores = filterScores(search, category.scores);
//     if (filteredScores.length > 0) {
//         return new Category(
//             category.name,
//             filteredScores
//         );
//     }
//     return null;
// }


// function filterScoreGroups__OLD(search: FilterValue, groups: ScoreGroup[]): ScoreGroup[] {
//     if (isEmpty(search)) {
//         // No filter, just return all.
//         return groups;
//     }
//     return groups
//         .map(group => filterScoreGroup(search, group))
//         .filter(group => group.isNotEmpty)
//         ;
// }

// function filterScoreGroup(search: FilterValue, group: ScoreGroup): ScoreGroup {
//     // If there are no labels, we search by title only.
//     if (search.labels.length == 0
//         // In that case, if the group is a match then we include all its children.
//         && group.searchableName.includes(search.text)) {
//         // Filter matches this group, so we return all children.
//         return group;
//     }

//     const filteredBranches = group.subGroups
//         .map(branch => filterScoreGroup(search, branch))
//         .filter(branch => branch.isNotEmpty);
//     const filteredLeaves = filterScores(search, group.scores);
//     return new ScoreGroup(group.name, group.parent, filteredLeaves, filteredBranches);
// }

export function filterScores(search: FilterValue, scores: Score[]): Score[] {
    if (isEmpty(search)) {
        // No filter, just return all.
        return scores;
    }
    let filtered = scores;
    let before = filtered;
    filtered = filterScoresByText(search.text, filtered);
    let second = filtered;
    filtered = filterScoresByLabels(search.labels, filtered);
    return filtered;
}

function filterScoresByText(searchText: string, scores: Score[]): Score[] {
    if (searchText.length === 0) {
        return scores;
    }
    return scores.filter(score => score.searchableName.includes(searchText));
}

function filterScoresByLabels(labels: LabelFilter[], scores: Score[]): Score[] {
    if (labels.length == 0) {
        return scores;
    }
    return scores.filter(score => {
        const matches = labels.map(label => ({
            name: label.name,
            value: label.value,
            onScore: score.labels.get(label.name),
        }));
        const unmatchedLabels = matches.filter(match => match.value != match.onScore);
        // const unmatchedLabels = labels.filter(label => score.labels.get(label.name) !== label.name);
        return unmatchedLabels.length === 0;
    });
}
