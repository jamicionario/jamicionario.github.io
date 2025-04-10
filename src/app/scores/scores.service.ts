import { Injectable } from '@angular/core';

export type Score = {
  id: number,
  title: string,
  description?: string,
};

@Injectable({
  providedIn: 'root'
})
export class ScoreService {
  getScores() : Score[] {
    return sampleScoreData;
  }

  getScore(scoreId: number): Score | undefined {
    return sampleScoreData.find(score => score.id === scoreId);
  }
}

const sampleScoreData: Score[] = [
  { id: 1, title: 'Fado Batido',
description:
`O fado batido era geralmente acompanhado, que não cantado, sendo a coreografia executada por um batedor que sapateava no centro de um pequeno círculo de homens e, aproximando-se sucessivamente dos colegas, desferia nestes golpes de peito e de pélvis (umbigadas).
A coreografia podia acelerar e desacelerar, o batedor podia ceder o seu lugar a outro, batiam-se vigorosamente palmas e davam-se gritos de excitação.
Havia batedores do fado que metiam a guitarra ou a viola de arame aos ombros.
Bordallo Pinheiro documenta com elevado grau de fidelidade cenas do bater do fado a que assistiu em tascas e bordeis de Lisboa.`
  },
  { id: 2, title: 'Pera Verde',
description: `Deste-me uma pêra verde
Estava a meio de amadurar
Pêra verde, minha verde pêra
Não me venhas enganar`
  },

  { id: 5, title: 'Repasseado',
description: `O repasseado é uma dança inspirada em recolhas que foram realizadas em Trás-os-Montes, e apresenta-se sob a forma de quadrilha, ou seja, dois pares frente a frente.
A coreografia divide-se em duas partes, A e B, que se repetem. A parte A começa com o bater de três palmas ao centro, seguida da deslocação da quadrilha no sentido anti horário - esta deslocação é feita com um movimento alternado, em direcção do par e o contrapar.
Na parte B, os dois pares ficam frente a frente e trocam de posição numa deslocação lateral.`
  },
];